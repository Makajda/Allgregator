using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Common;
using Allgregator.Spl.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Allgregator.Spl.ViewModels {
    public class ButimeViewModel : DataViewModelBase<DataBase<Bumined>> {
        private DelegateCommand<Butask> subCommand; public ICommand SubCommand => subCommand ??= new DelegateCommand<Butask>(Sub);
        private DelegateCommand<Butask> addCommand; public ICommand AddCommand => addCommand ??= new DelegateCommand<Butask>(Add);

        private void Sub(Butask butask) {
            if (butask != null && butask.Now > double.Epsilon) {
                var butime = Change(butask, -butask.Value / 2d);
                if (butime != null && butime.Value < double.Epsilon) {
                    butask.Butimes.Remove(butime);
                }
            }
        }

        private void Add(Butask butask) {
            Change(butask, butask.Value);
        }

        private Butime Change(Butask butask, double value) {
            if (butask == null) return null;//<-----------------------return

            var newDate = DateTimeOffset.Now;
            Butime butime = null;
            if (butask.Butimes == null)
                butask.Butimes = new List<Butime>();
            else
                butime = butask.Butimes.LastOrDefault(n => Givenloc.IsIncludedInTheInterval(newDate, n.Date));

            if (butime == null) {
                if (value > double.Epsilon) {
                    butime = new Butime { Date = newDate, Value = value };
                    butask.Butimes.Add(butime);
                }
            }
            else {
                butime.Date = newDate;
                var newValue = butime.Value + value;
                butime.Value = newValue > double.Epsilon ? newValue : 0d;
            }

            Data.Mined.LastWork = newDate;
            Data.Mined.IsNeedToSave = true;
            butask.Recalc();
            Data.Mined.RecalcMax();
            return butime;
        }
    }
}
