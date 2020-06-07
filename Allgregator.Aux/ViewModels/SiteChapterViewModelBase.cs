using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public abstract class SiteChapterViewModelBase<TItem, TView> : ChapterViewModelBase where TItem : IName {
        private readonly IRegionManager regionManager;
        private readonly ZipRepositoryBase<MinedBase<TItem>> minedRepository;
        public SiteChapterViewModelBase(
            string title,
            SiteOreServiceBase<TItem> oreService,
            ZipRepositoryBase<MinedBase<TItem>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(settings, eventAggregator) {
            Data.Title = title;
            OreService = oreService;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
        }

        public DataBase<MinedBase<TItem>> Data { get; } = new DataBase<MinedBase<TItem>>();
        public SiteOreServiceBase<TItem> OreService { get; private set; }
        protected override async Task Activate() {
            var view = typeof(TView).FullName;
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, view, parameters);
            await LoadMined();
        }
        protected override async Task Deactivate() => await SaveMined();
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
                await OreService.Retrieve(Data.Mined);
            }
        }

        private async Task LoadMined() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();
            }
        }

        private async Task SaveMined() {
            if (Data.Mined != null) {
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
