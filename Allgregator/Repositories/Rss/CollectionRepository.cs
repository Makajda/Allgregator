using Allgregator.Models.Rss;
using System.Collections.Generic;

namespace Allgregator.Repositories.Rss {
    public class CollectionRepository {
        public IEnumerable<Collection> GetCollections() {
            var collections = new List<Collection>();
            collections.Add(new Collection(1) { Name = "Первая коллекция" });
            collections.Add(new Collection(2) { Name = "Вторая коллекция" });
            collections.Add(new Collection(3) { Name = "Третья коллекция" });
            return collections;
        }
    }
}
