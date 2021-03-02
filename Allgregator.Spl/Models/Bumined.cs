using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Allgregator.Spl.Models {
    public class Bumined : BindableBase, IWatchSave {
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

        private double maxValue = double.MinValue;
        [JsonIgnore]
        public double MaxValue {
            get => maxValue;
            set => SetProperty(ref maxValue, value);
        }

        [JsonIgnore]
        public bool IsNeedToSave { get; set; }

        public void Recalc() {
            var max = double.MinValue;
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
                MaxValue = double.MinValue;
        }
    }
}
