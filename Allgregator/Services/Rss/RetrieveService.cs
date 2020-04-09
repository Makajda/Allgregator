using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Allgregator.Services.Rss {
    public class RetrieveService : IDisposable {
        private object syncNewRecos = new object();
        private object syncOldRecos = new object();
        private object syncErrors = new object();

        public List<Reco> NewRecos { get; } = new List<Reco>();
        public List<Reco> OldRecos { get; } = new List<Reco>();
        public List<Error> Errors { get; } = new List<Error>();

        public void Dispose() {
            NewRecos.Clear();
            OldRecos.Clear();
            Errors.Clear();
        }

        public void Production(Link link, DateTimeOffset acceptTime, DateTimeOffset cutoffTime, IEnumerable<Reco> outdated) {
            try {
                var uri = new Uri(link.XmlUrl);
                using var reader = XmlReader.Create(link.XmlUrl);
                var feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items) {
                    if (item.PublishDate > cutoffTime) {
                        var reco = GetRecoFromSyndication(feed, item);
                        if (item.PublishDate > acceptTime) {
                            if (outdated?.FirstOrDefault(n => n.Equals(reco)) == null) {
                                lock (syncNewRecos) {
                                    NewRecos.Add(reco);
                                    continue;
                                }
                            }
                        }

                        lock (syncOldRecos) {
                            OldRecos.Add(reco);
                        }
                    }
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
