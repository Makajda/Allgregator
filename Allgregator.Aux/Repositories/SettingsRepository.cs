using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Allgregator.Aux.Repository {
    public class SettingsRepository {
        private const string nameFile = "Settings.json";

        public Settings GetOrDefault() {
            Settings settings = null;

            try {
                settings = Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return settings ?? new Settings();
        }

        public void Save(Settings settings) {
            if (!Directory.Exists(Given.PathData))
                Directory.CreateDirectory(Given.PathData);
            var json = JsonSerializer.Serialize<Settings>(settings,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
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
