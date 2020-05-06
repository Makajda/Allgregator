using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Fin.Models {
    public class Mined : BindableBase {
        private DateTimeOffset lastRetrieve;
        public DateTimeOffset LastRetrieve {
            get => lastRetrieve;
            set => SetProperty(ref lastRetrieve, value);
        }

        private IList<Term> terms;
        public IList<Term> Terms {
            get => terms;
            set => SetProperty(ref terms, value);
        }

        private IEnumerable<Currency> currencies;
        public IEnumerable<Currency> Currencies {
            get => currencies;
            set => SetProperty(ref currencies, value);
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
