using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.ViewModels;
using Allgregator.Fin.Models;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly IRegionManager regionManager;
        private readonly RepositoryBase<Mined> minedRepository;
        private readonly RepositoryBase<Cured> curedRepository;
        private bool isSettings;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ZipRepositoryBase<Mined> minedRepository,
            RepositoryBase<Cured> curedRepository
            ) : base(settings, eventAggregator) {
            OreService = oreService;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
            this.curedRepository = curedRepository;
            minedRepository.SetNames(Module.Name);
            curedRepository.SetNames(Module.Name);
            chapterId = Module.Name;
        }

        public Data Data { get; } = new Data();
        public OreService OreService { get; private set; }
        protected override async Task Activate() {
            ViewActivate();
            await Load();
        }
        protected override async Task Deactivate() => await Save();
        protected override void Run() {
            isSettings = !isSettings;
            ViewActivate();
        }
        protected override void WindowClosing(CancelEventArgs args) {
            base.WindowClosing(args);
            AsyncHelper.RunSync(async () => await Save());
        }
        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await Load();
                await OreService.Retrieve(Data);
            }
        }

        private void ViewActivate() {
            var view = isSettings ? typeof(SettingsView).FullName : typeof(CurrencyView).FullName;
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, view, parameters);
        }

        private async Task Load() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();
            }

            if (Data.Cured == null) {
                Data.Cured = await curedRepository.GetOrDefault();
                if (Data.Cured.Currencies == null) {
                    Data.Cured.Currencies = new[] { "USD", "EUR", "GBP", "CHF", "CNY", "UAH" };
                }
            }
        }

        private async Task Save() {
            if (Data.Mined != null) {
                try {
                    await minedRepository.Save(Data.Mined);
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }

            if (Data.Cured != null) {
                try {
                    await curedRepository.Save(Data.Cured);
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
    }
}
