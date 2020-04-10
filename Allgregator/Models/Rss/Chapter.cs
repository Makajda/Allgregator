using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Allgregator.Models.Rss {
    public class Chapter : BindableBase {
        public int Id { get; set; }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ObservableCollection<Link> links;
        [JsonIgnore]
        public ObservableCollection<Link> Links {
            get => links;
            set => SetProperty(ref links, value);
        }

        private Mined mined;
        [JsonIgnore]
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }

        [JsonIgnore]
        public bool IsNeedToSaveLinks { get; set; }

        [JsonIgnore]
        public bool IsNeedToSaveMined { get; set; }
    }
}
