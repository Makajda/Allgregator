using Allgregator.Aux.Common;
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
    public class ButimeViewModel : DataViewModelBase<Data> {
        public ButimeViewModel() {
            Data.Butasks = new Obsefy<Butask> {
                new Butask { Name = "a", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now.AddDays(-1), Value = 100 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } },
                new Butask { Name = "b", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now.AddMinutes(-15), Value = 1 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } },
                new Butask { Name = "c", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now, Value = 98 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } },
                new Butask { Name = "d" },
                new Butask { Name = "e", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now, Value = 3 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } }
            };

            Data.Recalc();
        }

        private DelegateCommand<Butask> subCommand; public ICommand SubCommand => subCommand ??= new DelegateCommand<Butask>(n => Sub(n, 15));
        private DelegateCommand<Butask> add15Command; public ICommand Add15Command => add15Command ??= new DelegateCommand<Butask>(n => Add(n, 15));
        private DelegateCommand<Butask> add30Command; public ICommand Add30Command => add30Command ??= new DelegateCommand<Butask>(n => Add(n, 30));

        public new Data Data { get; } = new Data();
        
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


            butask.Recalc();
            Data.RecalcMax();
        }
    }
}
