using Allgregator.Aux.ViewModels;
using Allgregator.Fin.Models;
using Prism.Commands;

namespace Allgregator.Fin.ViewModels {
    public class SettingsViewModel : DataViewModelBase<Data> {
        public SettingsViewModel() {
            DeleteCommand = new DelegateCommand(Delete);
        }

        public DelegateCommand DeleteCommand { get; private set; }

        private void Delete() {
            Data.Mined.Terms = null;
        }
    }
}
