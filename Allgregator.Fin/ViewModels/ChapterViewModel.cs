using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Fin.Models;
using Allgregator.Fin.Repositories;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        private readonly IRegionManager regionManager;
        private readonly MinedRepository minedRepository;
        private bool isSettings;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            MinedRepository minedRepository
            ) : base(eventAggregator) {
            OreService = oreService;
            this.settings = settings;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
        }

        public Data Data { get; } = new Data();
        public OreService OreService { get; private set; }
        protected override string ChapterId => Module.Name;
        protected override async Task Activate() {
            await LoadMined();
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
                await OreService.Retrieve(Data.Mined, settings.FinStartDate, settings.FinCurrencies);
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
