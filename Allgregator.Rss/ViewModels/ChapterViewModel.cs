using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Prism.Events;
using Prism.Regions;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly RepoService repoService;
        private readonly ViewService viewService;

        public ChapterViewModel(
            OreService oreService,
            RepoService repoService,
            ViewService viewService,
            Settings settings,
            IEventAggregator eventAggregator
            ) : base(settings, eventAggregator) {
            OreService = oreService;
            this.repoService = repoService;
            this.viewService = viewService;

            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await repoService.LinkMoved(Data, n));
        }
        public Data Data { get; } = new Data();
        public OreService OreService { get; private set; }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            if (navigationContext.Parameters.TryGetValue(Common.Givenloc.ChapterIdParameter, out int id)) {
                Data.Id = id;
            }

            if (navigationContext.Parameters.TryGetValue(Common.Givenloc.ChapterNameParameter, out string title)) {
                Data.Title = title;
            }

            Data.IsNeedToSave = false;
            chapterId = $"{Module.Name}{Data.Id}";
            base.OnNavigatedTo(navigationContext);
        }

        protected override async Task Activate() {
            viewService.ActivateMainView(Data);
            await repoService.Load(Data);
        }
        protected override async Task Deactivate() => await repoService.Save(Data);

        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await repoService.Load(Data);
                await OreService.Retrieve(Data, settings.RssCutoffTime);
            }
        }

        protected override void WindowClosing(CancelEventArgs args) {
            base.WindowClosing(args);
            AsyncHelper.RunSync(async () => await repoService.Save(Data));
        }
    }
}
