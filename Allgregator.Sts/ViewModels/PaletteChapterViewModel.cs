using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Models;
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
                "https://docs.microsoft.com/en-gb/dotnet/api/system.windows.media.brushes?view=net-5.0",
                oreService,
                retrieveService,
                "Palette",
                Module.Name,
                typeof(PaletteView).FullName,
                null,
                minedRepository,
                regionManager,
                eventAggregator,
                settings) {
        }
    }
}
