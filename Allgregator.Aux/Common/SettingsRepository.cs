using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Allgregator.Aux.Common {
    public class SettingsRepository {
        private const string nameFile = "settings.json";

        public Settings GetOrDefault() {
            Settings retval = null;

            try {
                retval = Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? new Settings();
        }

        public void Save(Settings settings) {
            if (!Directory.Exists(Given.PathData))
                Directory.CreateDirectory(Given.PathData);
            var json = JsonSerializer.Serialize<Settings>(settings,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            var name = Path.Combine(Given.PathData, nameFile);
            File.WriteAllText(name, json);
        }

        private Settings Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = File.ReadAllText(name);
            return JsonSerializer.Deserialize<Settings>(json);
        }
    }
}
