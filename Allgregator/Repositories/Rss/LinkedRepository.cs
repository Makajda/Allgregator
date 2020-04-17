using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class LinkedRepository {
        private const string nameFile = "linked{0}.json";

        public async Task<Linked> GetOrDefault(int chapterId) {
            try {
                return await Get(chapterId);
            }
            catch (Exception e) {
                /*//TODO Log*/
            }

            return CreateDefault();
        }

        private async Task<Linked> Get(int chapterId) {
            var name = GetName(chapterId);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<Linked>(json);
        }

        public async Task Save(int chapterId, Linked linked) {
            var json = JsonSerializer.Serialize<Linked>(linked,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            var name = GetName(chapterId);
            await File.WriteAllTextAsync(name, json);
        }

        public void DeleteFile(int id) {
            var name = GetName(id);
            File.Delete(name);
        }

        private string GetName(int chapterId) {
            var name = Path.Combine(Given.PathData, string.Format(nameFile, chapterId));
            return name;
        }

        private Linked CreateDefault() {
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
    }
}