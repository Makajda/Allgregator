using Allgregator.Aux.Models;
using Prism.Mvvm;
using System.Text.Json.Serialization;

namespace Allgregator.Rss.Models {
    public class Data : BindableBase, IWatchSave {
        public int Id { get; set; }

        private string name;
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value, () => IsNeedToSave = true); }
        }

        private Linked linked;
        [JsonIgnore]
        public Linked Linked {
            get { return linked; }
            set { SetProperty(ref linked, value); }
        }

        private Mined mined;
        [JsonIgnore]
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
