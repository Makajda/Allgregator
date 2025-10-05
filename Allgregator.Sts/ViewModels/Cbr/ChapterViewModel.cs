using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Cbr.Models;
using Allgregator.Sts.Cbr.Services;
using Allgregator.Sts.Views.Cbr;
using Prism.Events;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Sts.ViewModels.Cbr {
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
            ) : base(eventAggregator, settings) {
            OreService = oreService;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
            this.curedRepository = curedRepository;
            minedRepository.SetNames(Module.Name, "CbrMined");
            curedRepository.SetNames(Module.Name, "CbrCured");
            Data.Title = "Cbr";
            chapterId = $"{Module.Name}Cbr";
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
                var cured = await curedRepository.GetOrDefault();
                if (cured.Currencies == null) {
                    cured.Currencies = new[] { "USD", "EUR", "GBP", "CHF", "CNY", "UAH" };
                }

                Data.Cured = cured;//after define currencies
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
