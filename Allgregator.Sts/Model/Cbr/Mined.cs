using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Sts.Cbr.Models {
    public class Mined : BindableBase, IWatchSave {
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

        private IEnumerable<Error> errors;
        public IEnumerable<Error> Errors {
            get => errors;
            set => SetProperty(ref errors, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
