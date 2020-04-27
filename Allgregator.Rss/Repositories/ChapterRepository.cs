using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    internal class ChapterRepository {
        private const string nameFile = "chapters.json";

        public async Task<IEnumerable<Chapter>> GetOrDefault() {
            IEnumerable<Chapter> retval = null;

            try {
                retval = await Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? CreateDefault();
        }

        internal async Task Save(IEnumerable<Chapter> chapters) {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = JsonSerializer.Serialize<IEnumerable<Chapter>>(chapters.OrderBy(n => n.Name),
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(name, json);
        }

        internal int GetNewId(IEnumerable<Chapter> chapters) {
            var newId = 1;
            foreach (var chapter in chapters.OrderBy(n => n.Id)) {
                var id = chapter.Id;
                if (id != newId) {
                    break;
                }

                newId++;
            }

            return newId;
        }

        private async Task<IEnumerable<Chapter>> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<IEnumerable<Chapter>>(json);
        }

        private IEnumerable<Chapter> CreateDefault() {
            return new List<Chapter>() {
                new Chapter() { Id = Given.TryChapter, Name = "Try" },
                new Chapter() { Id = 1, Name = "Other" }
            };
        }
    }
}
