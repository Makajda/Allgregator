using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Allgregator.Spl.Views;
using Prism.Events;
using Prism.Regions;

namespace Allgregator.Spl.ViewModels {
    internal class ButimeChapterViewModel : SimpleChapterViewModelBase<Butime> {
        public ButimeChapterViewModel(
            ZipRepositoryBase<MinedBase<Butime>> minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(
                "Times",
                Module.Name,
                typeof(ButimeView).FullName,
                typeof(ButimeSettingsView).FullName,
                minedRepository,
                regionManager,
                eventAggregator,
                settings) {
        }
    }
}
