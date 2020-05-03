using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Fin.Models;
using Allgregator.Fin.Repositories;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.ComponentModel;
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
        }

        public OreService OreService { get; private set; }
        public Data Data { get; } = new Data();

        protected override int ChapterId => Given.FinChapter;

        protected override async Task Activate() {
            await LoadMined();
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = typeof(CurrencyView).Name;
            var view = region.GetView(viewName);
            if (view == null) {
                region.Context = Data;
                view = container.Resolve<CurrencyView>();
                region.Add(view, viewName);
            }

            region.Activate(view);
        }

        protected override void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = Given.FinChapter;
            AsyncHelper.RunSync(async () => await SaveMined());
        }

        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await LoadMined();
                settings.FinStartDate = new DateTimeOffset(2020, 3, 18, 0, 0, 0, TimeSpan.Zero);
                await OreService.Retrieve(Data.Mined, settings.FinStartDate);
            }
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
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
    }
}
