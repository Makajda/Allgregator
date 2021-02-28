using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Allgregator.Spl.Repositories;
using Allgregator.Spl.Views;
using Prism.Events;
using Prism.Regions;
using System.Threading.Tasks;

namespace Allgregator.Spl.ViewModels {
    internal class ButimeChapterViewModel : SimpleChapterViewModelBase<Mined> {
        public ButimeChapterViewModel(
            ButimeRepository repository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(
                "Times",
                Module.Name,
                typeof(ButimeView).FullName,
                typeof(ButimeSettingsView).FullName,
                repository,
                regionManager,
                eventAggregator,
                settings) {
        }

        protected override async Task LoadMined() {
            await base.LoadMined();
            Data?.Mined?.Recalc();
        }
    }
}
