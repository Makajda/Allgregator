using Allgregator.Aux.Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Fin.Models {
    public class Mined : BindableBase {
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
