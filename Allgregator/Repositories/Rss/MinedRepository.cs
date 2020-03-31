using Allgregator.Common;
using Allgregator.Models.Rss;
using Newtonsoft.Json;
using System.IO;

namespace Allgregator.Repositories.Rss {
    public class MinedRepository {
        private const string nameFile = "mined{0}.json";
        public Mined GetMined(int collectionId) {
            var name = Path.Combine(Given.PathData, string.Format(nameFile, collectionId));
            if (File.Exists(name)) {
                var json = File.ReadAllText(name);
                return JsonConvert.DeserializeObject<Mined>(json);
            }

            return new Mined();
        }

        public void Save(int collectionId, Mined mined) {
            var json = JsonConvert.SerializeObject(mined, Formatting.Indented);
            var name = Path.Combine(Given.PathData, string.Format(nameFile, collectionId));
            File.WriteAllText(name, json);
        }
    }
}
