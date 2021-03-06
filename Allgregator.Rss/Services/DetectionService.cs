﻿using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Allgregator.Rss.Services {
    internal class DetectionService {
        private readonly WebService webService;
        private const int timeout = 45_000;
        internal DetectionService(
            WebService webService
            ) {
            this.webService = webService;
        }

        internal async Task SetAddress(Linked linked) {
            linked.CurrentState = LinksStates.Detection;
            var link = await GetLink(linked.Address);
            if (link != null) {
                Selected(linked, link);
            }
            else {
                var links = await GetLinks(linked.Address);
                if (links == null || links.Count == 0) {
                    links.Add(new Link() { Name = linked.Address, XmlUrl = linked.Address, HtmlUrl = linked.Address });
                }
                else {
                    links = links.Distinct(EqualityComparer<Link>.Default).OrderBy(n => n.XmlUrl.ToString().Length).ToList();
                }

                links.Add(new Link());
                linked.DetectedLinks = links;
                linked.CurrentState = LinksStates.Selection;
                linked.IsNeedToSave = true;
            }
        }

        internal void Selected(Linked linked, Link link) {
            if (link?.XmlUrl != null) {
                if (linked.Links == null) {
                    linked.Links = new ObservableCollection<Link>();
                }

                linked.Links.Insert(0, link);
                linked.Address = null;
                linked.DetectedLinks = null;
            }

            linked.CurrentState = LinksStates.Normal;
            linked.IsNeedToSave = true;
        }

        private async Task<Link> GetLink(string address) {
            Link link = null;
            if (!string.IsNullOrWhiteSpace(address)) {
                using var cancellationTokenSource = new CancellationTokenSource(timeout);
                try {
                    await Task.Run(() => {
                        try {
                            using var reader = XmlReader.Create(address);
                            var feed = SyndicationFeed.Load(reader);
                            link = new Link() {
                                Name = RegexUtilities.GetText(feed.Title?.Text),
                                XmlUrl = address,
                                HtmlUrl = address
                            };
                        }
                        catch (Exception) { }
                    }, cancellationTokenSource.Token);
                }
                catch (OperationCanceledException) { }
            }

            return link;
        }

        private async Task<IList<Link>> GetLinks(string address) {
            if (string.IsNullOrWhiteSpace(address)) {
                return null;
            }

            // 1-8 1.get html from addingUri
            // 2.find rsses into html
            var rsses = await GetRsses(new string[] { address });

            // 3.validation found rsses
            var links = await ValidationRsses(rsses);
            if (links.Count == 0) {
                // 4.get htmls from rsses
                // 5.find rsses into htmls
                // 6.validation rsses
                rsses = await GetRsses(rsses);
                links = await ValidationRsses(rsses);
                if (links.Count == 0) {
                    // 7.find additional uris
                    // 8.validation additional uris
                    rsses = GetAdditionalUris(address);
                    links = await ValidationRsses(rsses);
                }
            }

            return links;
        }

        private async Task<IList<Link>> ValidationRsses(IEnumerable<string> rsses) {
            var links = new List<Link>();
            object sync = new object();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            try {
                await Task.WhenAll(
                        Partitioner.Create(rsses).GetPartitions(5).Select(partition =>
                            Task.Run(async () => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        var link = await GetLink(partition.Current);
                                        if (link != null) {
                                            lock (sync) {
                                                links.Add(link);
                                            }
                                        }
                                    }
                                };
                            }, cancellationTokenSource.Token)));
            }
            catch (Exception) { }

            return links;
        }

        private async Task<IEnumerable<string>> GetRsses(IEnumerable<string> addresses) {
            var result = new List<string>();
            object sync = new object();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            try {
                await Task.WhenAll(
                        Partitioner.Create(addresses).GetPartitions(5).Select(partition =>
                            Task.Run(async () => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        var html = await webService.TryGetHtml(partition.Current);
                                        if (html != null) {
                                            var hrefs = RegexUtilities.GetHrefs(html);
                                            var rsses = hrefs.Where(n => n != null && (n.Contains("rss") || n.Contains("atom") || n.Contains("feed"))).Distinct();
                                            lock (sync) {
                                                result.AddRange(rsses);
                                            }
                                        }
                                    }
                                };
                            }, cancellationTokenSource.Token)));
            }
            catch (Exception) { }

            return result;
        }

        private List<string> GetAdditionalUris(string address) {
            var result = new List<string>();
            var adatas = GetSuffixes();
            foreach (var adata in adatas) {
                try {
                    var wAddress = new UriBuilder(address);
                    if (wAddress.Path.Length > 1) {
                        wAddress.Path = wAddress.Path.TrimEnd('/') + $"/{adata}";
                        wAddress.Query = null;
                        result.Add(wAddress.Uri.ToString());
                        wAddress = new UriBuilder(address);
                    }

                    wAddress.Path = adata;
                    wAddress.Query = null;
                    result.Add(wAddress.Uri.ToString());
                    result.Add(wAddress.Uri.ToString().Replace("http://", "https://"));
                }
                catch (Exception) { }
            }

            return result;
        }

        private IEnumerable<string> GetSuffixes() {
            return new string[] {
                "rss",
                "feed",
                "atom",
                "data/rss",
                "data/feed",
                "data/atom",
                "info/rss",
                "info/feed",
                "info/atom",
                "rss.xml",
                "feed.xml",
                "atom.xml",
                "public.rss",
                "public.feed",
                "public.atom",
                "news.rss",
                "news.feed",
                "news.atom",
                "rss.aspx",
                "feed.aspx",
                "atom.aspx",
                "feed/atom",
                "feed/rss",
                "feeds/posts/default",
                "posts.rss"
            };
        }
    }
}
