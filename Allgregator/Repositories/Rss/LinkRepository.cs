using Allgregator.Models.Rss;
using System.Collections.Generic;

namespace Allgregator.Repositories.Rss {
    public class LinkRepository {
        public IEnumerable<Link> GetLinks(int collectionId) {
            var links = new List<Link>();

            if (collectionId == 1)
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml",
                    Name = "BBC News - Health",
                    XmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml"
                });
            else if (collectionId == 2)
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.foxnews.com/foxnews/sports",
                    Name = "FOX News",
                    XmlUrl = "http://feeds.foxnews.com/foxnews/sports"
                });
            else
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.reuters.com/news/artsculture",
                    Name = "Reuters: Arts",
                    XmlUrl = "http://feeds.reuters.com/news/artsculture"
                });

            return links;
        }
    }
}
