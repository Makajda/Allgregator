using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Allgregator.Models.Rss {
    public class Collection : BindableBase {
        public readonly int Id;

        public Collection(int id) {
            Id = id;
        }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ObservableCollection<Link> links;
        public ObservableCollection<Link> Links {
            get => links;
            set => SetProperty(ref links, value);
        }

        private Mined mined;
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }
    }
}
