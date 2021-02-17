using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Allgregator.Spl.Views;
using Prism.Events;
using Prism.Regions;
using System.Threading.Tasks;

namespace Allgregator.Spl.ViewModels {
    internal class ButimeChapterViewModel : ChapterViewModelBase {
        private readonly IRegionManager regionManager;
        public ButimeChapterViewModel(
            IEventAggregator eventAggregator,
            Settings settings,
            IRegionManager regionManager
            ) : base(settings, eventAggregator) {
            var itemName = "Times";
            this.regionManager = regionManager;
            Data.Title = itemName;
            chapterId = $"{Module.Name}{itemName}";
        }
        public DataBase<MinedBase<Butime>> Data { get; } = new DataBase<MinedBase<Butime>>();

        protected override async Task Activate() {
            regionManager.RequestNavigate(Given.MainRegion, typeof(ButimeView).FullName);
        }

        protected override async Task Deactivate() {
        }

        protected override async Task Update() {
        }
    }
}
