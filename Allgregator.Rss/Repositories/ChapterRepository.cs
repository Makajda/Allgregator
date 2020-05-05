using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Allgregator.Rss.Repositories {
    public class ChapterRepository {
        private const string nameFile = "rssChapters.json";

        public IEnumerable<Data> GetOrDefault() {
            IEnumerable<Data> retval = null;

            try {
                retval = Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? CreateDefault();
        }

        public void Save(IEnumerable<Data> chapters) {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = JsonSerializer.Serialize<IEnumerable<Data>>(chapters.OrderBy(n => n.Name),
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            File.WriteAllText(name, json);
        }

        public Data GetNewChapter(IEnumerable<Data> chapters, string name) {
            var newId = 1;
            foreach (var chapter in chapters.OrderBy(n => n.Id)) {
                var id = chapter.Id;
                if (id != newId) {
                    break;
                }

                newId++;
            }

            return new Data() { Id = newId, Name = name };
        }

        private IEnumerable<Data> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = File.ReadAllText(name);
            return JsonSerializer.Deserialize<IEnumerable<Data>>(json);
        }

        private IEnumerable<Data> CreateDefault() {
            return new List<Data>() {
                new Data() { Id = Given.TryChapter, Name = "Try" },
                new Data() { Id = 1, Name = "Other" }
            };
        }
    }
}
