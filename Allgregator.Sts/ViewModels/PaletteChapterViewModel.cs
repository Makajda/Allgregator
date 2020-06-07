using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Repositories;
using Allgregator.Sts.Services;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;

namespace Allgregator.Sts.ViewModels {
    internal class PaletteChapterViewModel : SiteChapterViewModelBase<PaletteColor, PaletteView> {
        public PaletteChapterViewModel(
            SiteOreServiceBase<PaletteColor> s,
            PaletteOreService oreService,
            PaletteMinedRepository minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base("Palette", oreService, minedRepository, eventAggregator, regionManager, settings) {
        }

        protected override string ChapterId => $"{Module.Name}Palette";
    }
}
