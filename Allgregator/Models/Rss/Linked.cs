using Allgregator.Common;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Allgregator.Models.Rss {
    public class Linked : BindableBase {
        private ObservableCollection<Link> links;
        public ObservableCollection<Link> Links {
            get => links;
            set => SetProperty(ref links, value);
        }

        private RssLinksStates currentState;
        public RssLinksStates CurrentState {
            get => currentState;
            set => SetProperty(ref currentState, value);
        }

        private string address;
        public string Address {
            get => address;
            set => SetProperty(ref address, value);
        }

        private IEnumerable<Link> detectedLinks;
        public IEnumerable<Link> DetectedLinks {
            get => detectedLinks;
            set => SetProperty(ref detectedLinks, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
