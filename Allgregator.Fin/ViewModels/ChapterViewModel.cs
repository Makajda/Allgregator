using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Fin.Models;
using Allgregator.Fin.Repositories;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        private readonly IRegionManager regionManager;
        private readonly MinedRepository minedRepository;
        private readonly IContainerExtension container;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IContainerExtension container,
            MinedRepository minedRepository
            ) : base(eventAggregator) {
            OreService = oreService;
            this.settings = settings;
            this.regionManager = regionManager;
            this.container = container;
            this.minedRepository = minedRepository;

            SettingCommand = new DelegateCommand(ViewActivate<SettingsView>);
        }

        public DelegateCommand SettingCommand { get; private set; }
        public OreService OreService { get; private set; }
        public Data Data { get; } = new Data();
        protected override int ChapterId => Given.FinChapter;

        protected override async Task Activate() {
            await LoadMined();
            ViewActivate<CurrencyView>();
        }

        protected override async Task Deactivate() => await SaveMined();
        protected override void Run() => ViewActivate<CurrencyView>();

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
                await OreService.Retrieve(Data.Mined, settings.FinStartDate);
            }
        }

        private void ViewActivate<TView>() {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = typeof(TView).Name;
            var view = region.GetView(viewName);
            if (view == null) {
                region.Context = Data;
                view = container.Resolve<TView>();
                region.Add(view, viewName);
            }

            region.Activate(view);
        }

        private async Task LoadMined() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();

                if (Data.Mined.Currencies == null) {
                    Data.Mined.Currencies = Fin.Common.Given.CurrencyNames.Select(n => new Currency() { Key = n });
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
