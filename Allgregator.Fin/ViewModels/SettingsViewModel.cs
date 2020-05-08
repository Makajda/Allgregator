using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Fin.Models;
using Prism.Commands;

namespace Allgregator.Fin.ViewModels {
    public class SettingsViewModel : DataViewModelBase<Data> {
        public SettingsViewModel(
            Settings settings
            ) {
            this.Settings = settings;

            DeleteCommand = new DelegateCommand(Delete);
        }

        public DelegateCommand DeleteCommand { get; private set; }
        public Settings Settings { get; private set; }

        private void Delete() {
            Data.Mined.Terms = null;
        }
    }
}
