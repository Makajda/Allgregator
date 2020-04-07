using Allgregator.Common;
using Allgregator.Models.Rss;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class ChapterRepository {
        private const string nameFile = "chapters.json";
        public async Task<IEnumerable<Chapter>> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<IEnumerable<Chapter>>(json);
        }

        public async Task Save(IEnumerable<Chapter> chapters) {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = JsonSerializer.Serialize<IEnumerable<Chapter>>(chapters,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(name, json);
        }
    }
}
