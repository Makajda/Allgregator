using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Prism.Events;
using Prism.Navigation.Regions;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public abstract class SiteChapterViewModelBase<TItem> : SimpleChapterViewModelBase<MinedBase<TItem>> {
        public SiteChapterViewModelBase(
            string itemName,
            string moduleName,
            string viewMain,
            string viewSettings,
            string address,
            SiteOreServiceBase<TItem> oreService,
            SiteRetrieveServiceBase<TItem> retrieveService,
            RepositoryBase<MinedBase<TItem>> minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(itemName, moduleName, viewMain, viewSettings, minedRepository, regionManager, eventAggregator, settings) {
            minedRepository.SetNames(moduleName, $"{itemName}Mined");
            OreService = oreService;
            oreService.Initialize(address, retrieveService);
        }

        public SiteOreServiceBase<TItem> OreService { get; private set; }

        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await LoadMined();
                await OreService.Retrieve(Data.Mined);
            }
        }
    }
}
