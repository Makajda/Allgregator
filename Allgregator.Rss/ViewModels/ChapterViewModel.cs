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
        private readonly RepoService repoService;
        private readonly ViewService viewService;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            OreService oreService,
            RepoService repoService,
            ViewService viewService,
            Settings settings,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) : base(eventAggregator) {
            OreService = oreService;
            this.repoService = repoService;
            this.viewService = viewService;
            this.settings = settings;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<ChapterViews?>(async (view) => await ChangeView(view));
            MoveCommand = new DelegateCommand(Move);

            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await repoService.LinkMoved(Data, n));
            //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(id => repoService.DeleteFiles(id));
        }
        public DelegateCommand<ChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public OreService OreService { get; private set; }
        public Data Data { get; } = new Data();

        private ChapterViews currentView = ChapterViews.SettingsView;//todo
        public ChapterViews CurrentView {
            get { return currentView; }
            set { SetProperty(ref currentView, value); }
        }

        protected override int ChapterId => Data.Id;
        protected override async Task Activate() => await CurrentViewChanged();
        protected override async Task Deactivate() => await repoService.Save(Data);
        protected override void Run() {
            var recos = CurrentView switch
            {
                ChapterViews.NewsView => Data.Mined?.NewRecos,
                ChapterViews.OldsView => Data.Mined?.OldRecos,
                _ => null
            };

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

        private async Task ChangeView(ChapterViews? view) {
            if (IsActive) {
                CurrentView = CurrentView = view ?? ChapterViews.NewsView;
                await CurrentViewChanged();
            }
        }

        private async Task CurrentViewChanged() {
            viewService.ManageMainViews(CurrentView, Data);
            await repoService.Load(Data, CurrentView == ChapterViews.LinksView);
        }

        private void OpenReal() {
            var mined = Data.Mined;
            if (mined != null) {
                if (CurrentView == ChapterViews.NewsView) {
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
