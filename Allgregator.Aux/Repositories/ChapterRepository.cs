using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public class ChapterRepository {
        private const string nameFile = "chapters.json";

        public async Task<IEnumerable<ChapterBase>> GetOrDefault() {
            IEnumerable<ChapterBase> retval = null;

            try {
                retval = await Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? CreateDefault();
        }

        public async Task Save(IEnumerable<ChapterBase> chapters) {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = JsonSerializer.Serialize<IEnumerable<ChapterBase>>(chapters.OrderBy(n => n.Name),
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(name, json);
        }

        public int GetNewId(IEnumerable<ChapterBase> chapters) {
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

        private async Task<IEnumerable<ChapterBase>> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<IEnumerable<ChapterBase>>(json);
        }

        private IEnumerable<ChapterBase> CreateDefault() {
            return new List<ChapterBase>() {
                new ChapterBase() { Id = Given.TryChapter, Spec=Given.SpecRss, Name = "Try" },
                new ChapterBase() { Id = 1, Spec=Given.SpecRss, Name = "Other" },
                new ChapterBase() { Id = 2, Spec=Given.SpecFin, Name = "Cbr" }
            };
        }
    }
}
