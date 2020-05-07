using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Allgregator.Rss.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        private readonly RepoService repoService;
        private readonly ViewService viewService;
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            OreService oreService,
            RepoService repoService,
            ViewService viewService,
            Settings settings,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) : base(eventAggregator) {
            OreService = oreService;
            this.repoService = repoService;
            this.viewService = viewService;
            this.settings = settings;
            this.dialogService = dialogService;
            this.regionManager = regionManager;

            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await repoService.LinkMoved(Data, n));
        }
        public OreService OreService { get; private set; }
        public Data Data { get; } = new Data();

        protected override int ChapterId => Data.Id;
        protected override async Task Activate() {
            //viewService.ActivateMainView(Data);

            var p = new NavigationParameters();
            p.Add("Data", Data);
            //regionManager.Regions[Aux.Common.Given.RegionMain].Context = Data;
            regionManager.RequestNavigate(Aux.Common.Given.RegionMain, "MainView", p);
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
            if (IsActive) settings.CurrentChapterId = Data.Id;
            AsyncHelper.RunSync(async () => await repoService.Save(Data));
        }
    }
}
