using Allgregator.Common;
using Allgregator.Models.Rss;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Allgregator.Repositories.Rss {
    public class ChapterRepository {
        private const string nameFile = "chapters.json";
        public IEnumerable<Chapter> GetChapters() {
            var name = Path.Combine(Given.PathData, nameFile);
            if (File.Exists(name)) {
                var json = File.ReadAllText(name);
                return JsonConvert.DeserializeObject<IEnumerable<Chapter>>(json);
            }
            else {
                var chapters = new List<Chapter>();
                chapters.Add(new Chapter(1) { Name = "Основной" });
                chapters.Add(new Chapter(2) { Name = "Второй" });
                chapters.Add(new Chapter(3) { Name = "Третий" });
                return chapters;
            }
        }

        public void Save(IEnumerable<Chapter> chapters) {
            var json = JsonConvert.SerializeObject(chapters, Formatting.Indented);
            var name = Path.Combine(Given.PathData, nameFile);
            File.WriteAllText(name, json);
        }
    }
}
