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
    internal class PaletteChapterViewModel : SiteChapterViewModelBase<PaletteColor, PaletteView> {
        public PaletteChapterViewModel(
            SiteOreServiceBase<PaletteColor> s,
            PaletteOreService oreService,
            ZipRepositoryBase<MinedBase<PaletteColor>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(oreService, minedRepository, eventAggregator, regionManager, settings) {
            minedRepository.SetNames(Module.Name, "MinedPalette");
        }

        protected override string ChapterId => $"{Module.Name}Palette";
    }
}
