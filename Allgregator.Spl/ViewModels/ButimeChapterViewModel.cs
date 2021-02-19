using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Events;
using Prism.Regions;
using System.Threading.Tasks;

namespace Allgregator.Spl.ViewModels {
    internal class ButimeChapterViewModel : ChapterViewModelBase {
        public ButimeChapterViewModel(
            ZipRepositoryBase<MinedBase<Butime>> minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(settings, eventAggregator) {
            //"Times",
            //Module.Name,
            //typeof(ButimeView).FullName,
            //typeof(ButimeSettingsView).FullName,
            //minedRepository,
            //regionManager,
            //eventAggregator,
            //settings) {
        }

        protected override Task Activate() {
            return Task.CompletedTask;
        }

        protected override Task Deactivate() {
            return Task.CompletedTask;
        }

        protected override Task Update() {
            return Task.CompletedTask;
        }
    }
}
