using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Commands;

namespace Allgregator.Spl.ViewModels {
    public class ButimeSettingsViewModel : DataViewModelBase<DataBase<MinedBase<Butime>>> {
        public ButimeSettingsViewModel() {
            DeleteCommand = new DelegateCommand(Delete);
        }

        public DelegateCommand DeleteCommand { get; private set; }

        private void Delete() {
            //todo Data.Mined.Terms = null;
        }
    }
}
