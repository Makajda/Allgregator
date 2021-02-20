using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Allgregator.Spl.Models {
    public class Mined : BindableBase, IWatchSave {
        private DateTimeOffset lastWork;
        public DateTimeOffset LastWork {
            get => lastWork;
            set => SetProperty(ref lastWork, value);
        }

        private Obsefy<Butask> butasks;
        public Obsefy<Butask> Butasks {
            get => butasks;
            set => SetProperty(ref butasks, value);
        }

        private int maxValue = int.MinValue;
        [JsonIgnore]
        public int MaxValue {
            get => maxValue;
            set => SetProperty(ref maxValue, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }

        public void Recalc() {
            var max = int.MinValue;
            if (Butasks != null) {
                foreach (var butask in Butasks) {
                    butask.Recalc();
                    if (max < butask.Total)
                        max = butask.Total;
                }
            }

            MaxValue = max;
        }

        public void RecalcMax() {
            if (Butasks != null)
                MaxValue = Butasks.Max(n => n.Total);
            else
                MaxValue = int.MinValue;
        }
    }

    public class Butask : BindableBase {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private Color color;
        public Color Color {
            get => color;
            set => SetProperty(ref color, value);
        }

        private List<Butime> butimes;
        public List<Butime> Butimes {
            get => butimes;
            set => SetProperty(ref butimes, value);
        }

        private int now;
        [JsonIgnore]
        public int Now {
            get => now;
            private set => SetProperty(ref now, value);
        }

        private int today;
        [JsonIgnore]
        public int Today {
            get => today;
            private set => SetProperty(ref today, value);
        }

        private int total;
        [JsonIgnore]
        public int Total {
            get => total;
            private set => SetProperty(ref total, value);
        }

        public void Recalc() {
            var total = 0;
            var today = 0;
            var now = 0;
            if (Butimes != null) {
                foreach (var butime in Butimes) {
                    total += butime.Value;
                    if (butime.Date.Date == DateTime.Today) {
                        today += butime.Value;
                        if (butime.Date - DateTimeOffset.Now < TimeSpan.FromMinutes(15d))
                            now += butime.Value;
                    }
                }

                Total = total;
                Today = today;
                Now = now;
            }
        }
    }

    public class Butime {
        public DateTimeOffset Date { get; set; }
        public int Value { get; set; }
    }
}
