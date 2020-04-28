using Prism.Mvvm;
using System;

namespace Allgregator.Fin.Models {
    public class Currency : BindableBase {
        private DateTimeOffset date;
        public DateTimeOffset Date {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        //todo убрать setProperty
        private decimal usd;
        public decimal Usd {
            get { return usd; }
            set { SetProperty(ref usd, value); }
        }

        private decimal eur;
        public decimal Eur {
            get { return eur; }
            set { SetProperty(ref eur, value); }
        }

        private decimal gbp;
        public decimal Gbp {
            get { return gbp; }
            set { SetProperty(ref gbp, value); }
        }

        private decimal chf;
        public decimal Chf {
            get { return chf; }
            set { SetProperty(ref chf, value); }
        }

        private decimal cny;
        public decimal Cny {
            get { return cny; }
            set { SetProperty(ref cny, value); }
        }

        private decimal uah;
        public decimal Uah {
            get { return uah; }
            set { SetProperty(ref uah, value); }
        }
    }
}
