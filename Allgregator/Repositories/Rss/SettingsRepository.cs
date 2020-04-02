using Allgregator.Common;
using Allgregator.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Allgregator.Repositories.Rss {
    public class SettingsRepository {
        private const string nameFile = "settings.json";
        public Settings Get() {
            try {
                var name = Path.Combine(Given.PathData, nameFile);
                var json = File.ReadAllText(name);
                return JsonConvert.DeserializeObject<Settings>(json);
            }
            catch (Exception) {
                return new Settings();
            }
        }

        public void Save(Settings settings) {
            try {
                if (!Directory.Exists(Given.PathData))
                    Directory.CreateDirectory(Given.PathData);

                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                var name = Path.Combine(Given.PathData, nameFile);
                File.WriteAllText(name, json);
            }
            catch (Exception) { /*TODO Log*/ }
        }
    }
}
