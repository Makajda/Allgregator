using Prism.Mvvm;
using System;

namespace Allgregator.Fin.Models {
    public class Currency : BindableBase {
        private DateTimeOffset date;
        public DateTimeOffset Date {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private string country;
        public string Country {
            get { return country; }
            set { SetProperty(ref country, value); }
        }

        private decimal val;
        public decimal Val {
            get { return val; }
            set { SetProperty(ref val, value); }
        }
    }
}
