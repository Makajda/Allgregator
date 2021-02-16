using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Events;
using System.Threading.Tasks;

namespace Allgregator.Spl.ViewModels {
    internal class ButimeChapterViewModel : ChapterViewModelBase {
        public ButimeChapterViewModel(
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(settings, eventAggregator) {
            var itemName = "Times";
            Data.Title = itemName;
            chapterId = $"{Module.Name}{itemName}";
        }
        public DataBase<MinedBase<Butime>> Data { get; } = new DataBase<MinedBase<Butime>>();

        protected override async Task Activate() {
        }

        protected override async Task Deactivate() {
        }

        protected override async Task Update() {
        }
    }
}
