using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Allgregator.Fin.Models {
    public class Cured : BindableBase, IWatchSave {

        private DateTimeOffset startDate = DateTimeOffset.Now.Date.Date.AddMonths(-1);
        public DateTimeOffset StartDate {
            get { return startDate; }
            set { SetProperty(ref startDate, value, () => IsNeedToSave = true); }
        }

        public IEnumerable<string> Currencies { get; set; }

        public IEnumerable<string> Offs { get; set; }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }
    }
}
