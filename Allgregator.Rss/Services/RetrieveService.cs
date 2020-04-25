﻿using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Allgregator.Rss.Services {
    internal class RetrieveService : IDisposable {
        private readonly object syncRecos = new object();
        private readonly object syncErrors = new object();

        internal List<Reco> Recos { get; } = new List<Reco>();
        internal List<Error> Errors { get; } = new List<Error>();

        public void Dispose() {
            Recos.Clear();
            Errors.Clear();
        }

        internal void Production(Link link, DateTimeOffset cutoffTime) {
            try {
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
