using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly IEventAggregator eventAggregator;
        private readonly ChapterService chapterService;
        private readonly ViewsService viewsService;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            Chapter chapter,
            OreService oreService,
            ChapterService chapterService,
            ViewsService viewsService,
            Settings settings,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            Chapter = chapter;
            OreService = oreService;
            this.chapterService = chapterService;
            this.viewsService = viewsService;
            this.settings = settings;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<RssChapterViews?>(async (view) => await ChangeView(view));
            OpenCommand = new DelegateCommand(Open);
            MoveCommand = new DelegateCommand(Move);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(async () => await chapterService.Save(Chapter)));
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(CurrentChapterChanged);
            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await chapterService.LinkMoved(Chapter, n));
            eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(id => chapterService.DeleteFiles(id));
        }

        public DelegateCommand<RssChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public OreService OreService { get; private set; }
        public Chapter Chapter { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        private RssChapterViews currentView;// = RssChapterViews.LinksView;//todo
        public RssChapterViews CurrentView {
            get => currentView;
            private set => SetProperty(ref currentView, value);
        }

        public void Activate() {
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Chapter);
            settings.RssChapterId = Chapter.Id;
        }

        private async Task ChangeView(RssChapterViews? view) {
            if (IsActive) {
                CurrentView = CurrentView = view ?? RssChapterViews.NewsView;
                await CurrentViewChanged();
            }
        }

        private async Task CurrentViewChanged() {
            viewsService.ManageMainViews(CurrentView, Chapter);
            await chapterService.Load(Chapter, CurrentView == RssChapterViews.LinksView);
        }

        private async void CurrentChapterChanged(Chapter chapter) {
            IsActive = chapter.Id == Chapter.Id;
            if (IsActive) {
                await CurrentViewChanged();
            }
            else {
                await chapterService.Save(Chapter);
            }
        }

        private void Open() {
            if (IsActive && CurrentView != RssChapterViews.LinksView) {
                var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
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
            else {
                Activate();
            }
        }

        private void OpenReal() {
            var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
            if (recos != null) {
                foreach (var reco in recos) WindowUtilities.Run(reco.Uri.ToString());
            }
        }

        private void Move() {
            if (Chapter?.Mined?.NewRecos != null && Chapter.Mined?.OldRecos != null) {
                Chapter.Mined.IsNeedToSave = true;
                var cache = Chapter.Mined.NewRecos.Reverse();
                foreach (var reco in cache) {
                    Chapter.Mined.NewRecos.Remove(reco);
                    Chapter.Mined.OldRecos.Insert(0, reco);
                }
            }
        }

        private async void Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await chapterService.Load(Chapter);
                await OreService.Retrieve(Chapter, settings.RssCutoffTime);
            }
        }
    }
}
