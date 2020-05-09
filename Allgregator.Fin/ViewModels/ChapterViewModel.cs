using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Fin.Models;
using Allgregator.Fin.Repositories;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        private readonly IRegionManager regionManager;
        private readonly MinedRepository minedRepository;
        private readonly OreService oreService;
        private bool isSettings;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            MinedRepository minedRepository
            ) : base(eventAggregator) {
            this.oreService = oreService;
            this.settings = settings;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
        }

        public Data Data { get; } = new Data();
        protected override int ChapterId => Given.FinChapter;
        public override OreServiceBase OreService => oreService;
        public override IEnumerable<Error> Errors => Data.Mined?.Errors;
        protected override async Task Activate() {
            await LoadMined();
            RaisePropertyChanged(nameof(Errors));
            ViewActivate();
        }
        protected override async Task Deactivate() => await SaveMined();
        protected override void Run() {
            isSettings = !isSettings;
            ViewActivate();
        }
        protected override void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = ChapterId;
            AsyncHelper.RunSync(async () => await SaveMined());
        }
        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await LoadMined();
                await oreService.Retrieve(Data.Mined, settings.FinStartDate);
                RaisePropertyChanged(nameof(Errors));
            }
        }

        private void ViewActivate() {
            var view = isSettings ? typeof(SettingsView).FullName : typeof(CurrencyView).FullName;
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, view, parameters);
        }

        private async Task LoadMined() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();

                if (Data.Mined.Currencies == null) {
                    Data.Mined.Currencies = Fin.Common.Givenloc.CurrencyNames.Select(n => new Currency() { Key = n });
                }
            }
        }

        private async Task SaveMined() {
            if (Data.Mined != null && Data.Mined.IsNeedToSave) {
                try {
                    await minedRepository.Save(Data.Mined);
                    Data.Mined.IsNeedToSave = false;
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
    }
}
