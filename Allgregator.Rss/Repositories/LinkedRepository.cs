using Allgregator.Aux.Repositories;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using System.Collections.ObjectModel;

namespace Allgregator.Rss.Repositories {
    internal class LinkedRepository : RepositoryBase<Linked> {
        protected override Linked CreateDefault(string id) {
            if (id == Givenloc.GetChapterId(Givenloc.TryDataId)) {
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
                        HtmlUrl = "http://feeds.foxnews.com/foxnews/sports",
                        Name = "FOX News",
                        XmlUrl = "http://feeds.foxnews.com/foxnews/sports"
                    },
                    new Link() {
                        HtmlUrl = "http://feeds.reuters.com/news/artsculture",
                        Name = "Reuters: Arts",
                        XmlUrl = "http://feeds.reuters.com/news/artsculture"
                    }
                };

                return new Linked() { Links = links };
            }

            return new Linked();
        }
    }
}