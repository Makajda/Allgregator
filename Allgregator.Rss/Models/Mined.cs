using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Allgregator.Rss.Models {
    public class Mined : BindableBase {
        private DateTimeOffset lastRetrieve;
        public DateTimeOffset LastRetrieve {
            get => lastRetrieve;
            set => SetProperty(ref lastRetrieve, value, () => IsNeedToSave = true);
        }

        private DateTimeOffset acceptTime;
        public DateTimeOffset AcceptTime {
            get => acceptTime;
            set => SetProperty(ref acceptTime, value, () => IsNeedToSave = true);
        }

        private ObservableCollection<Reco> newRecos;
        public ObservableCollection<Reco> NewRecos {
            get => newRecos;
            set => SetProperty(ref newRecos, value);
        }

        private ObservableCollection<Reco> oldRecos;
        public ObservableCollection<Reco> OldRecos {
            get => oldRecos;
            set => SetProperty(ref oldRecos, value);
        }

        private IEnumerable<Error> errors;
        public IEnumerable<Error> Errors {
            get => errors;
            set => SetProperty(ref errors, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
