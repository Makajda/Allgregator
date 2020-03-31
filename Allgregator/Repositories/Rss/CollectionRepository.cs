using Allgregator.Common;
using Allgregator.Models.Rss;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Allgregator.Repositories.Rss {
    public class CollectionRepository {
        private const string nameFile = "collections.json";
        public IEnumerable<Collection> GetCollections() {
            var name = Path.Combine(Given.PathData, nameFile);
            if (File.Exists(name)) {
                var json = File.ReadAllText(name);
                return JsonConvert.DeserializeObject<IEnumerable<Collection>>(json);
            }
            else {
                var collections = new List<Collection>();
                collections.Add(new Collection(1) { Name = "Основной" });
                return collections;
            }
        }

        public void Save(IEnumerable<Collection> collections) {
            var json = JsonConvert.SerializeObject(collections, Formatting.Indented);
            var name = Path.Combine(Given.PathData, nameFile);
            File.WriteAllText(name, json);
        }
    }
}
