using Allgregator.Aux.Repositories;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using System.Collections.ObjectModel;

namespace Allgregator.Rss.Repositories {
    internal class LinkedRepository : RepositoryBase<Linked> {
        public LinkedRepository() { SetNames(Module.Name); }
        protected override Linked CreateDefault(int id) {
            if (id == Givenloc.TryNewsId) {
                var links = new ObservableCollection<Link>() {
                    new Link() {
                        HtmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml",
                        Name = "BBC News - Health",
                        XmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml"
                    },
                    new Link() {
                        HtmlUrl = "http://feeds.skynews.com/feeds/rss/business.xml",
                        Name = "Business News - Markets reports and financial news from Sky",
                        XmlUrl = "http://feeds.skynews.com/feeds/rss/business.xml"
                    },
                    new Link() {
                        HtmlUrl = "http://rss.cnn.com/rss/edition_technology.rss",
                        Name = "CNN.com - Technology",
                        XmlUrl = "http://rss.cnn.com/rss/edition_technology.rss"
                    },
                    new Link() {
                        HtmlUrl = "https://www.lepoint.fr/",
                        Name = "Le Point - Actualité",
                        XmlUrl = "https://www.lepoint.fr/rss.xml"
                    }
                };
                return new Linked() { Links = links };
            }
            else if (id == Givenloc.TrySportId) {
                var links = new ObservableCollection<Link>() {
                    new Link() {
                        HtmlUrl = "http://feeds.foxnews.com/foxnews/sports",
                        Name = "FOX News",
                        XmlUrl = "http://feeds.foxnews.com/foxnews/sports"
                    },
                    new Link() {
                        HtmlUrl = "https://www.sportschau.de/",
                        Name = "Sportschau",
                        XmlUrl = "https://www.sportschau.de//sportschauindex100~_type-rss.feed"
                    },
                    new Link() {
                        HtmlUrl = "https://www.football-italia.net/news",
                        Name = "Football Italia",
                        XmlUrl = "https://www.football-italia.net/rss.xml"
                    }
                };
                return new Linked() { Links = links };
            }
            else if (id == Givenloc.TryGameId) {
                var links = new ObservableCollection<Link>() {
                    new Link() {
                        HtmlUrl = "https://www.gamespot.com/",
                        Name = "Gamespot",
                        XmlUrl = "https://www.gamespot.com/feeds/news/"
                    },
                    new Link() {
                        HtmlUrl = "https://news.ea.com/",
                        Name = "EA",
                        XmlUrl = "https://news.ea.com/feeds/press_release/all/rss.xml"
                    }
                };
                return new Linked() { Links = links };
            }
            else if (id == Givenloc.TryProgId) {
                var links = new ObservableCollection<Link>() {
                    new Link() {
                        HtmlUrl = "Visual Studio Blog",
                        Name = "https://devblogs.microsoft.com/visualstudio/",
                        XmlUrl = "https://devblogs.microsoft.com/visualstudio/rss"
                    },
                    new Link() {
                        HtmlUrl = "https://csharpdigest.net/",
                        Name = "Weekly C# Digest",
                        XmlUrl = "http://feeds.feedburner.com/digest-csharp"
                    }
                };
                return new Linked() { Links = links };
            }

            return new Linked();
        }
    }
}