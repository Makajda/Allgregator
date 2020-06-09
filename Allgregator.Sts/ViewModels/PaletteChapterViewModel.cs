using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Services;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;

namespace Allgregator.Sts.ViewModels {
    internal class PaletteChapterViewModel : SiteChapterViewModelBase<PaletteColor> {
        public PaletteChapterViewModel(
            SiteOreServiceBase<PaletteColor> oreService,
            PaletteRetrieveService retrieveService,
            ZipRepositoryBase<MinedBase<PaletteColor>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(
                "Palette",
                Module.Name,
                typeof(PaletteView).FullName,
                "",
                oreService,
                retrieveService,
                minedRepository,
                eventAggregator,
                regionManager,
                settings) {
        }
    }
}
