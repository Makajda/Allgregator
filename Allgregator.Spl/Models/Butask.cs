using Allgregator.Spl.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Allgregator.Spl.Models {
    public class Butask : BindableBase {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value, () => IsNeedToSave = true);
        }

        private Color color = Colors.GreenYellow;
        public Color Color {
            get => color;
            set => SetProperty(ref color, value, () => IsNeedToSave = true);
        }

        private double valueReal = 30d;
        public double Value {
            get => valueReal;
            set => SetProperty(ref valueReal, value, () => IsNeedToSave = true);
        }

        private List<Butime> butimes;
        public List<Butime> Butimes {
            get => butimes;
            set => SetProperty(ref butimes, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }

        private double now;
        [JsonIgnore]
        public double Now {
            get => now;
            private set => SetProperty(ref now, value);
        }

        private double today;
        [JsonIgnore]
        public double Today {
            get => today;
            private set => SetProperty(ref today, value);
        }

        private double total;
        [JsonIgnore]
        public double Total {
            get => total;
            private set => SetProperty(ref total, value);
        }

        public void Recalc() {
            var total = 0d;
            var today = 0d;
            var now = 0d;
            var newDate = DateTimeOffset.Now;
            var newToday = DateTime.Today;
            if (Butimes != null) {
                foreach (var butime in Butimes) {
                    total += butime.Value;
                    if (butime.Date.Date == newToday) {
                        today += butime.Value;
                        if (Givenloc.IsIncludedInTheInterval(butime.Date, newDate))
                            now += butime.Value;
                    }
                }

                Total = total;
                Today = today;
                Now = now;
            }
        }
    }

}
