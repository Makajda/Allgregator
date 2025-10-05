using Allgregator.Aux.Common;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Allgregator.Rss.Repositories {
    public class ChapterRepository {
        private const string nameFile = "RssChapters.json";

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
            var json = JsonSerializer.Serialize(chapters,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
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

            return new Data() { Id = newId, Title = name };
        }

        private IEnumerable<Data> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = File.ReadAllText(name);
            return JsonSerializer.Deserialize<IEnumerable<Data>>(json);
        }

        private IEnumerable<Data> CreateDefault() {
            return new List<Data>() {
                new Data() { Id = Givenloc.TryNewsId, Title = "News" },
                new Data() { Id = Givenloc.TrySportId, Title = "Sport" },
                new Data() { Id = Givenloc.TryGameId, Title = "Game" },
                new Data() { Id = Givenloc.TryProgId, Title = "Prog" },
            };
        }
    }
}
