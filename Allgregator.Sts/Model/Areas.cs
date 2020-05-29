using Allgregator.Aux.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Sts.Model {
    public class Areas : IWatchSave {
        public IEnumerable<Area> Items { get; set; }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
