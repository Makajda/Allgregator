using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Cbr.Models;
using Prism.Commands;

namespace Allgregator.Sts.ViewModels.Cbr {
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
