using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Allgregator.Services.Rss {
    public class RetrieveService : IDisposable {
        private object syncRecos = new object();
        private object syncErrors = new object();

        public List<Reco> Recos { get; } = new List<Reco>();
        public List<Error> Errors { get; } = new List<Error>();

        public void Dispose() {
            Recos.Clear();
            Errors.Clear();
        }

        public void Production(Link link, DateTimeOffset cutoffTime) {
            try {
                var uri = new Uri(link.XmlUrl);
                using var reader = XmlReader.Create(link.XmlUrl);
                var feed = SyndicationFeed.Load(reader);
                var recos = new List<Reco>();
                foreach (var item in feed.Items) {
                    if (item.PublishDate > cutoffTime) {
                        var reco = GetRecoFromSyndication(feed, item);
                        recos.Add(reco);
                    }
                }

                lock (syncRecos) {
                    Recos.AddRange(recos);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                lock (syncErrors) {
                    Errors.Add(new Error() { Link = link.Name, Message = exception.Message });
                }
            }
        }

        private Reco GetRecoFromSyndication(SyndicationFeed feed, SyndicationItem item) {
            var imageUri = feed.ImageUrl;
            if (imageUri == null) {
                imageUri = RegexUtilities.GetImage(item.Summary?.Text);
            }

            var itemUri = item.Links == null || item.Links.Count == 0 ? null : item.Links[0]?.Uri;

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
