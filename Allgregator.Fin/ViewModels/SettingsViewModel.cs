using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Fin.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Allgregator.Fin.ViewModels {
    public class SettingsViewModel : BindableBase {
        private readonly DialogService dialogService;
        public SettingsViewModel(
            Settings settings,
            IRegionManager regionManager,
            DialogService dialogService
            ) {
            if (regionManager.Regions[Given.MainRegion].Context is Data data) {
                Data = data;
            }

            this.dialogService = dialogService;
            this.Settings = settings;

            DeleteCommand = new DelegateCommand(Delete);
        }

        public DelegateCommand DeleteCommand { get; private set; }
        public Settings Settings { get; private set; }
        public Data Data { get; private set; }

        private void Delete() {
            Data.Mined.Terms = null;
        }
    }
}
