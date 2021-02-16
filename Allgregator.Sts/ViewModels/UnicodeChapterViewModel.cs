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
    internal class UnicodeChapterViewModel : SiteChapterViewModelBase<UnicodeArea> {
        public UnicodeChapterViewModel(
            SiteOreServiceBase<UnicodeArea> oreService,
            UnicodeRetrieveService retrieveService,
            ZipRepositoryBase<MinedBase<UnicodeArea>> minedRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            Settings settings
            ) : base(
                "Unicode",
                Module.Name,
                typeof(UnicodeView).FullName,
                "https://unicode.org/charts/",
                oreService,
                retrieveService,
                minedRepository,
                eventAggregator,
                regionManager,
                settings) {
        }
    }
}
