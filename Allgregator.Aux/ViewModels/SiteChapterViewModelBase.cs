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
    public abstract class SiteChapterViewModelBase<TItem> : ChapterViewModelBase where TItem : IName {
        private readonly string viewName;
        private readonly IRegionManager regionManager;
        private readonly RepositoryBase<MinedBase<TItem>> minedRepository;
        public SiteChapterViewModelBase(
            string itemName,
            string moduleName,
            string viewName,
            string address,
            SiteOreServiceBase<TItem> oreService,
            SiteRetrieveServiceBase<TItem> retrieveService,
            RepositoryBase<MinedBase<TItem>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(settings, eventAggregator) {
            this.viewName = viewName;
            OreService = oreService;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
            minedRepository.SetNames(moduleName, $"{itemName}Mined");
            Data.Title = itemName;
            chapterId = $"{moduleName}{itemName}";
            oreService.Initialize(address, retrieveService);
        }

        public DataBase<MinedBase<TItem>> Data { get; } = new DataBase<MinedBase<TItem>>();
        public SiteOreServiceBase<TItem> OreService { get; private set; }

        protected override async Task Activate() {
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, viewName, parameters);
            await LoadMined();
        }
        protected override async Task Deactivate() => await SaveMined();
        protected override void WindowClosing(CancelEventArgs args) {
            base.WindowClosing(args);
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
