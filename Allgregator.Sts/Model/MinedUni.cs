using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Sts.Model {
    public class MinedUni : BindableBase, IWatchSave {
        private DateTimeOffset lastRetrieve;
        public DateTimeOffset LastRetrieve {
            get => lastRetrieve;
            set => SetProperty(ref lastRetrieve, value);
        }

        private IEnumerable<AreaUni> areas;
        public IEnumerable<AreaUni> Areas {
            get => areas;
            set => SetProperty(ref areas, value);
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
