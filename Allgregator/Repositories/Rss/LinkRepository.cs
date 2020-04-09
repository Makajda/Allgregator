using Allgregator.Common;
using Allgregator.Models.Rss;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class LinkRepository {
        private const string nameFile = "links{0}.json";
        public async Task<IEnumerable<Link>> Get(int chapterId) {
            var name = GetName(chapterId);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<IEnumerable<Link>>(json);
        }

        public async Task Save(int chapterId, IEnumerable<Link> links) {
            var json = JsonSerializer.Serialize<IEnumerable<Link>>(links,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            var name = GetName(chapterId);
            await File.WriteAllTextAsync(name, json);
        }

        private string GetName(int chapterId) {
            var name = Path.Combine(Given.PathData, string.Format(nameFile, chapterId));
            return name;
        }

        public static IEnumerable<Link> CreateDefault() {
            return new List<Link>() {
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
        }
    }
}