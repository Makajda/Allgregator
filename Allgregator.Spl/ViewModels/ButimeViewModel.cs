using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Events;
using System.ComponentModel;
using System.Linq;

namespace Allgregator.Spl.ViewModels {
    public class ButimeViewModel : DataViewModelBase<DataBase<MinedBase<Butime>>> {
        private readonly Settings settings;

        public ButimeViewModel(Settings settings, IEventAggregator eventAggregator) {
            this.settings = settings;
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);

            Butasks = new Obsefy<Butask>();
            Butasks.Add(new Butask { Name = "1", Value = 19 });
            Butasks.Add(new Butask { Name = "2", Value = 10 });
            Butasks.Add(new Butask { Name = "3", Value = 1 });
            Butasks.Add(new Butask { Name = "3", Value = 5 });
            MaxValue = Butasks.Max(n => n.Value);
        }

        private Obsefy<Butask> butasks;
        public Obsefy<Butask> Butasks {
            get { return butasks; }
            set { SetProperty(ref butasks, value); }
        }

        private int maxValue;
        public int MaxValue {
            get { return maxValue; }
            set { SetProperty(ref maxValue, value); }
        }

        private void WindowClosing(CancelEventArgs obj) {
        }
    }
}
