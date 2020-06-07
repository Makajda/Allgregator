using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Repositories;
using Allgregator.Sts.Services;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;

namespace Allgregator.Sts.ViewModels {
    internal class UnicodeChapterViewModel : SiteChapterViewModelBase<UnicodeArea, UnicodeView> {
        public UnicodeChapterViewModel(
            UnicodeOreService oreService,
            UnicodeMinedRepository minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base("Unicode", oreService, minedRepository, eventAggregator, regionManager, settings) {
        }

        protected override string ChapterId => $"{Module.Name}Unicode";
    }
}
