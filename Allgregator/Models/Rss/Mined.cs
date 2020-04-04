using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Allgregator.Models.Rss {
    public class Mined : BindableBase {
        private DateTimeOffset lastRetrieve;
        private DateTimeOffset acceptTime;
        private DateTimeOffset cutoffTime;
        private ObservableCollection<Reco> newRecos;
        private ObservableCollection<Reco> oldRecos;
        private IEnumerable<Error> errors;

        public DateTimeOffset LastRetrieve {
            get => lastRetrieve;
            set => SetProperty(ref lastRetrieve, value);
        }

        public DateTimeOffset AcceptTime {
            get => acceptTime;
            set => SetProperty(ref acceptTime, value);
        }

        public DateTimeOffset CutoffTime {
            get => cutoffTime;
            set => SetProperty(ref cutoffTime, value);
        }

        public ObservableCollection<Reco> NewRecos {
            get => newRecos;
            set => SetProperty(ref newRecos, value);
        }
        public ObservableCollection<Reco> OldRecos {
            get => oldRecos;
            set => SetProperty(ref oldRecos, value);
        }

        public IEnumerable<Error> Errors {
            get => errors;
            set => SetProperty(ref errors, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
