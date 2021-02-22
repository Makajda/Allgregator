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
    public class ButimeViewModel : DataViewModelBase<DataBase<Mined>> {
        //Main
        private DelegateCommand<Butask> subCommand; public ICommand SubCommand => subCommand ??= new DelegateCommand<Butask>(n => Sub(n, 15));
        private DelegateCommand<Butask> add15Command; public ICommand Add15Command => add15Command ??= new DelegateCommand<Butask>(n => Add(n, 15));
        private DelegateCommand<Butask> add30Command; public ICommand Add30Command => add30Command ??= new DelegateCommand<Butask>(n => Add(n, 30));

        private void Sub(Butask butask, int value) {
            if (butask != null && butask.Now >= value) {
                Add(butask, -value);
            }
        }

        private void Add(Butask butask, int value) {
            if (butask == null) return;//<-----------------------return

            var newDate = DateTimeOffset.Now;
            Butime butime = null;
            if (butask.Butimes == null)
                butask.Butimes = new List<Butime>();
            else
                butime = butask.Butimes.LastOrDefault(n => Math.Abs((newDate - n.Date).TotalSeconds) < Givenloc.NewDateInterval * 60);

            if (butime == null)
                butask.Butimes.Add(new Butime { Date = newDate, Value = value });
            else {
                butime.Date = newDate;
                butime.Value += value;
            }

            Data.Mined.LastWork = newDate;
            Data.Mined.IsNeedToSave = true;
            butask.Recalc();
            Data.Mined.RecalcMax();
        }

        //Settings
    }
}
