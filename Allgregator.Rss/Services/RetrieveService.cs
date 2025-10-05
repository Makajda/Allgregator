using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Allgregator.Rss.Services {
    internal class RetrieveService : RetrieveServiceBase<Link, Reco> {
        public DateTimeOffset CutoffTime { get; set; }

        public override void Production(Link link) {
            try {
                using var reader = XmlReader.Create(link.XmlUrl);
                var feed = SyndicationFeed.Load(reader);
                var recos = new List<Reco>();
                foreach (var item in feed.Items) {
                    if (item.PublishDate > CutoffTime) {
                        var reco = GetRecoFromSyndication(feed, item);
                        recos.Add(reco);
                    }
                }

                lock (syncItems) {
                    Items.AddRange(recos);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                lock (syncErrors) {
                    Errors.Add(new Error() { Source = link.Name, Message = Aux.Common.ExceptionHelper.GetMessage(exception) });
                }
            }
        }

        private Reco GetRecoFromSyndication(SyndicationFeed feed, SyndicationItem item) {
            var imageUri = feed.ImageUrl ?? RegexUtilities.GetImage(item.Summary?.Text);
            var itemUri = item.Links == null || item.Links.Count == 0 ? null : item.Links[0]?.Uri.ToString();

            var reco = new Reco() {
                ImageUri = imageUri,
                Uri = itemUri,
                FeedTitle = RegexUtilities.GetText(feed.Title?.Text),
                ItemTitle = RegexUtilities.GetText(item.Title?.Text),
                SummaryHtml = item.Summary?.Text,
                PublishDate = item.PublishDate
            };

            return reco;
        }
    }
}
