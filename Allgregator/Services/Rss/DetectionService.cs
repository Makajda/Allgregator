using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Allgregator.Services.Rss {
    public class DetectionService {
        private const int timeout = 45_000;

        public async Task SetAddress(Linked linked) {
            linked.CurrentState = RssLinksStates.Detection;
            var link = await GetLink(linked.Address);
            if (link != null) {
                Selected(linked, link);
            }
            else {
                linked.DetectedLinks = await GetLinks(linked.Address);
                linked.CurrentState = linked.DetectedLinks == null ? RssLinksStates.Normal : RssLinksStates.Selection;
                linked.IsNeedToSave = true;
            }
        }

        public void Selected(Linked linked, Link link) {
            if (link?.XmlUrl != null) {
                linked.Links.Insert(0, link);
                linked.Address = null;
                linked.DetectedLinks = null;
            }

            linked.CurrentState = RssLinksStates.Normal;
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
                catch (Exception) { }
            }

            return link;
        }

        private async Task<IEnumerable<Link>> GetLinks(string address) {
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
                    rsses = GetAdditionalUris(address);

                    // 8.validation additional uris
                    links = await ValidationRsses(rsses);
                }
            }

            if (links.Count == 0) {
                links.Add(new Link() { Name = address, XmlUrl = address, HtmlUrl = address });
            }
            else {
                links = links.Distinct(new LinkComparer()).OrderBy(n => n.XmlUrl.ToString().Length).ToList();
            }

            links.Add(new Link());
            return links;
        }

        private async Task<List<Link>> ValidationRsses(IEnumerable<string> rsses) {
            var links = new List<Link>();
            object sync = new object();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            try {
                await Task.WhenAll(rsses.Select(n => Task.Run(async () => {
                    var link = await GetLink(n);
                    if (link != null) {
                        lock (sync) {
                            links.Add(link);
                        }
                    }
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
                await Task.WhenAll(addresses.Select(n => Task.Run(async () => {
                    var html = await GetHtml(n);
                    if (html != null) {
                        var hrefs = RegexUtilities.GetHrefs(html);
                        var rsses = hrefs.Where(n => n != null && (n.Contains("rss") || n.Contains("atom") || n.Contains("feed"))).Distinct();
                        lock (sync) {
                            result.AddRange(rsses);
                        }
                    }
                }, cancellationTokenSource.Token)));
            }
            catch (Exception) { }

            return result;
        }

        private async Task<string> GetHtml(string address) {
            using (var httpClient = new HttpClient()) {
                string html;
                try {
                    html = await httpClient.GetStringAsync(address);
                }
                catch (Exception) {
                    html = null;
                }

                return html;
            }
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

        class LinkComparer : EqualityComparer<Link> {
            public override bool Equals([AllowNull] Link x, [AllowNull] Link y) {
                if (x == null || y == null)
                    return true;

                return x.Equals(y);
            }

            public override int GetHashCode([DisallowNull] Link obj) {
                return obj.GetHashCode();
            }
        }
    }
}
