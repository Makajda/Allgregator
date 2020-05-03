using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        private readonly IEventAggregator eventAggregator;
        private readonly ChapterService chapterService;
        private readonly ViewService viewService;
        private readonly DialogService dialogService;
        private ChapterViews currentView = ChapterViews.LinksView;//todo

        public ChapterViewModel(
            OreService oreService,
            ChapterService chapterService,
            ViewService viewService,
            Settings settings,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) : base(eventAggregator) {
            OreService = oreService;
            this.chapterService = chapterService;
            this.viewService = viewService;
            this.settings = settings;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<ChapterViews?>(async (view) => await ChangeView(view));
            MoveCommand = new DelegateCommand(Move);

            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await chapterService.LinkMoved(Data, n));
            //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(id => chapterService.DeleteFiles(id));
        }
        public DelegateCommand<ChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public OreService OreService { get; private set; }
        public Data Data { get; } = new Data();

        protected override async Task OnChapterChanged(int chapterId, bool wasActive) {
            IsActive = chapterId == Data.Id;
            if (wasActive) {
                await chapterService.Save(Data);
            }

            if (IsActive) {
                await CurrentViewChanged();
            }
        }

        protected override Task Open() {
            if (IsActive) {
                if (currentView != ChapterViews.LinksView) {
                    var recos = currentView == ChapterViews.NewsView ? Data.Mined?.NewRecos : Data.Mined?.OldRecos;
                    if (recos != null) {
                        var count = recos.Count;
                        if (count > settings.RssMaxOpenTabs) {
                            dialogService.Show($"{count}?", OpenReal, 72d);
                        }
                        else {
                            OpenReal();
                        }
                    }
                }
            }
            else {
                eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Data.Id);
            }

            return Task.CompletedTask;
        }

        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await chapterService.Load(Data);
                await OreService.Retrieve(Data, settings.RssCutoffTime);
            }
        }

        protected override void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = Data.Id;
            AsyncHelper.RunSync(async () => await chapterService.Save(Data));
        }

        private async Task ChangeView(ChapterViews? view) {
            if (IsActive) {
                currentView = currentView = view ?? ChapterViews.NewsView;
                await CurrentViewChanged();
            }
        }

        private async Task CurrentViewChanged() {
            viewService.ManageMainViews(currentView, Data);
            await chapterService.Load(Data, currentView == ChapterViews.LinksView);
        }

        private void OpenReal() {
            var mined = Data.Mined;
            if (mined != null) {
                if (currentView == ChapterViews.NewsView) {
                    if (mined.NewRecos != null && mined.OldRecos != null) {
                        foreach (var reco in mined.NewRecos.Reverse()) {
                            WindowUtilities.Run(reco.Uri);
                            mined.OldRecos.Insert(0, reco);
                        }
                    }

                    mined.NewRecos.Clear();
                    mined.AcceptTime = mined.LastRetrieve;
                }
                else {
                    if (mined.OldRecos != null) {
                        foreach (var reco in mined.OldRecos) WindowUtilities.Run(reco.Uri);
                    }
                }
            }
        }

        private void Move() {
            var mined = Data.Mined;
            if (mined != null && mined.NewRecos != null && mined.OldRecos != null) {
                mined.IsNeedToSave = true;
                foreach (var reco in mined.NewRecos.Reverse()) mined.OldRecos.Insert(0, reco);
                mined.NewRecos.Clear();
                mined.AcceptTime = mined.LastRetrieve;
            }
        }
    }
}
