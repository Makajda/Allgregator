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
    public abstract class SiteChapterViewModelBase<TItem> : SimpleChapterViewModelBase<TItem> where TItem : IName {
        public SiteChapterViewModelBase(
            string address,
            SiteOreServiceBase<TItem> oreService,
            SiteRetrieveServiceBase<TItem> retrieveService,
            string itemName,
            string moduleName,
            string viewMain,
            string viewSettings,
            RepositoryBase<MinedBase<TItem>> minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(itemName, moduleName, viewMain, viewSettings, minedRepository, regionManager, eventAggregator, settings) {
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
