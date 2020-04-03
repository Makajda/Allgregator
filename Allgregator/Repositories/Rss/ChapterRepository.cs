﻿using Allgregator.Common;
using Allgregator.Models.Rss;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Allgregator.Repositories.Rss {
    public class ChapterRepository {
        private const string nameFile = "chapters.json";
        public IEnumerable<Chapter> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            if (File.Exists(name)) {
                var json = File.ReadAllText(name);
                return JsonConvert.DeserializeObject<IEnumerable<Chapter>>(json);
            }
            else {
                var chapters = new List<Chapter>();
                chapters.Add(new Chapter(4) { Name = "Четвёртая" });
                chapters.Add(new Chapter(1) { Name = "Основная" });
                chapters.Add(new Chapter(2) { Name = "Вторая" });
                chapters.Add(new Chapter(5) { Name = "Пятая" });
                chapters.Add(new Chapter(3) { Name = "Третья" });
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
