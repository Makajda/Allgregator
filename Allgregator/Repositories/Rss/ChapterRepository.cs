using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class ChapterRepository {
        private const string nameFile = "chapters.json";

        public async Task<IEnumerable<Chapter>> GetOrDefault() {
            try {
                return await Get();
            }
            catch (Exception e) {
                /*//TODO Log*/
            }

            return CreateDefault();
        }

        private async Task<IEnumerable<Chapter>> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<IEnumerable<Chapter>>(json);
        }

        public async Task Save(IEnumerable<Chapter> chapters) {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = JsonSerializer.Serialize<IEnumerable<Chapter>>(chapters.OrderBy(n => n.Name),
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(name, json);
        }

        private IEnumerable<Chapter> CreateDefault() {
            return new List<Chapter>() {
                new Chapter() { Id = 1, Name = "Base" },
                new Chapter() { Id = 2, Name = "Sport" },
                new Chapter() { Id = 3, Name = "Art" }
            };
        }
    }
}
