using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Allgregator.Spl.ViewModels {
    public class ButimeViewModel : DataViewModelBase<DataBase<MinedBase<Butime>>> {
        private readonly Settings settings;

        public ButimeViewModel(Settings settings, IEventAggregator eventAggregator) {
            this.settings = settings;
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);

            Butasks = new Obsefy<Butask> {
                new Butask { Name = "a", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now.AddDays(-1), Value = 100 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } },
                new Butask { Name = "b", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now.AddMinutes(-15), Value = 1 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } },
                new Butask { Name = "c", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now, Value = 98 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } },
                new Butask { Name = "d" },
                new Butask { Name = "e", Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now, Value = 3 }, new Butime { Date = DateTimeOffset.Now, Value = 2 } } }
            };

            var max = int.MinValue;
            foreach (var butask in Butasks) {
                butask.Recalc();
                if (max < butask.Total)
                    max = butask.Total;
            }

            MaxValue = max;
        }

        private DelegateCommand<Butask> addCommand; public ICommand AddCommand => addCommand ??= new DelegateCommand<Butask>(Add);

        private Obsefy<Butask> butasks;
        public Obsefy<Butask> Butasks {
            get => butasks;
            set => SetProperty(ref butasks, value);
        }

        private int maxValue;
        public int MaxValue {
            get => maxValue;
            set => SetProperty(ref maxValue, value);
        }

        private void WindowClosing(CancelEventArgs obj) {
        }

        private void Add(Butask butask) {//todo 3 commands
            if (butask != null) {
                if (Butasks == null)
                    Butasks = new Obsefy<Butask>();

                if (butask.Butimes == null)
                    butask.Butimes = new List<Butime> { new Butime { Date = DateTimeOffset.Now } };//todo

                butask.Butimes[0].Value += 30;//todo анализ на нужное значение, если в пределах 15 минут (добавить в givenLoc) или добавить
                butask.Recalc();
                MaxValue = Butasks.Max(n => n.Total);
            }
        }
    }
}
