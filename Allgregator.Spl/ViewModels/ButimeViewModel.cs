using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Events;
using System.ComponentModel;

namespace Allgregator.Spl.ViewModels {
    public class ButimeViewModel : DataViewModelBase<DataBase<MinedBase<Butime>>> {
        private readonly Settings settings;
        public ButimeViewModel(Settings settings, IEventAggregator eventAggregator) {
            this.settings = settings;
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
        }

        private void WindowClosing(CancelEventArgs obj) {
        }
    }
}
