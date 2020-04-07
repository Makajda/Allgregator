using Allgregator.Common;
using Allgregator.Models;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class SettingsRepository {
        private const string nameFile = "settings.json";
        public Settings Get() {
            try {
                var name = Path.Combine(Given.PathData, nameFile);
                var json = File.ReadAllText(name);
                return JsonSerializer.Deserialize<Settings>(json);
            }
            catch (Exception) {
                return new Settings();
            }
        }

        public async Task Save(Settings settings) {
            if (!Directory.Exists(Given.PathData))
                Directory.CreateDirectory(Given.PathData);
            var json = JsonSerializer.Serialize<Settings>(settings,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            var name = Path.Combine(Given.PathData, nameFile);
            await File.WriteAllTextAsync(name, json);
        }
    }
}
