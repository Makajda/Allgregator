using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;

namespace Allgregator.Sts.ViewModels {
    internal class UnicodeChapterViewModel : SiteChapterViewModelBase<UnicodeArea> {
        public UnicodeChapterViewModel(
            SiteOreServiceBase<UnicodeArea> oreService,
            ZipRepositoryBase<MinedBase<UnicodeArea>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(
                "Unicode",
                Module.Name,
                typeof(UnicodeView).FullName,
                oreService,
                minedRepository,
                eventAggregator,
                regionManager,
                settings) {
        }
    }
}
