using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Allgregator.Models.Rss {
    public class Chapter : BindableBase {
        public Chapter(int id) {
            Id = id;
        }

        public int Id { get; private set; }

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
    }
}
