using Allgregator.Common;
using Allgregator.Models.Rss;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Allgregator.Repositories.Rss {
    public class LinkRepository {
        private const string nameFile = "links{0}.json";
        public IEnumerable<Link> Get(int chapterId) {
            var name = Path.Combine(Given.PathData, string.Format(nameFile, chapterId));
            if (File.Exists(name)) {
                var json = File.ReadAllText(name);
                return JsonConvert.DeserializeObject<IEnumerable<Link>>(json);
            }
            else {
                var links = new List<Link>();
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml",
                    Name = "BBC News - Health",
                    XmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml"
                });
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.skynews.com/feeds/rss/business.xml",
                    Name = "Business News - Markets reports and financial news from Sky",
                    XmlUrl = "http://feeds.skynews.com/feeds/rss/business.xml"
                });
                links.Add(new Link() {
                    HtmlUrl = "http://rss.cnn.com/rss/edition_technology.rss",
                    Name = "CNN.com - Technology",
                    XmlUrl = "http://rss.cnn.com/rss/edition_technology.rss"
                });
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.foxnews.com/foxnews/sports",
                    Name = "FOX News",
                    XmlUrl = "http://feeds.foxnews.com/foxnews/sports"
                });
                links.Add(new Link() {
                    HtmlUrl = "http://feeds.reuters.com/news/artsculture",
                    Name = "Reuters: Arts",
                    XmlUrl = "http://feeds.reuters.com/news/artsculture"
                });
                return links;
            }
        }

        public void Save(int chapterId, IEnumerable<Link> links) {
            var json = JsonConvert.SerializeObject(links, Formatting.Indented);
            var name = Path.Combine(Given.PathData, string.Format(nameFile, chapterId));
            File.WriteAllText(name, json);
        }
    }
}
