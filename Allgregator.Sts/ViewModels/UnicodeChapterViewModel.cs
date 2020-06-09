using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Services;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;

namespace Allgregator.Sts.ViewModels {
    internal class UnicodeChapterViewModel : SiteChapterViewModelBase<UnicodeArea, UnicodeView> {
        public UnicodeChapterViewModel(
            UnicodeOreService oreService,
            ZipRepositoryBase<MinedBase<UnicodeArea>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(oreService, minedRepository, eventAggregator, regionManager, settings) {
            minedRepository.SetNames(Module.Name, "MinedUnicode");
        }

        protected override string ChapterId => $"{Module.Name}Unicode";
    }
}
