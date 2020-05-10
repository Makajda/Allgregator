using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Sts.Model {
    public class Mined : BindableBase {
        private DateTimeOffset lastRetrieve;
        public DateTimeOffset LastRetrieve {
            get => lastRetrieve;
            set => SetProperty(ref lastRetrieve, value);
        }

        private IEnumerable<Symbol> symbols;
        public IEnumerable<Symbol> Symbols {
            get => symbols;
            set => SetProperty(ref symbols, value);
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
