using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly ChapterService chapterService;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            Chapter chapter,
            OreService oreService,
            ChapterService chapterService,
            Settings settings,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            Chapter = chapter;
            OreService = oreService;
            this.chapterService = chapterService;
            this.settings = settings;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<RssChapterViews?>(async (view) => await ChangeView(view));
            OpenCommand = new DelegateCommand(OpenAll);
            MoveCommand = new DelegateCommand(MoveAll);
            UpdateCommand = new DelegateCommand(Update);
            CancelUpdateCommand = new DelegateCommand(OreService.CancelRetrieve);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(async () => await chapterService.Save(Chapter)));
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(CurrentChapterChanged);
            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await chapterService.LinkMoved(Chapter, n));
            eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(id => chapterService.DeleteFiles(id));
        }

        public DelegateCommand<RssChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        public DelegateCommand CancelUpdateCommand { get; private set; }
        public OreService OreService { get; private set; }
        public Chapter Chapter { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            private set => SetProperty(ref isActive, value);
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

        public void Deactivate() {
            IsActive = false;
            var region = regionManager.Regions[Given.MainRegion];
            var viewControl = region.GetView(CurrentView.ToString());
            if (viewControl != null) region.Deactivate(viewControl);
        }

        private async Task ChangeView(RssChapterViews? view) {
            if (IsActive) {
                CurrentView = CurrentView = view ?? RssChapterViews.NewsView;
                await CurrentViewChanged();
            }
        }

        private async Task CurrentViewChanged() {
            var region = regionManager.Regions[Given.MainRegion];
            var viewControl = region.GetView(CurrentView.ToString());
            if (viewControl != null) region.Activate(viewControl);
            await chapterService.Load(Chapter, CurrentView);
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

        private void OpenAll() {
            if (IsActive && CurrentView != RssChapterViews.LinksView) {
                var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
                var count = recos.Count;
                if (count > settings.RssMaxOpenTabs) {
                    dialogService.Show($"{count}?", OpenReal, 72d);
                }
                else {
                    OpenReal();
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

        private void MoveAll() {
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
            if (!OreService.IsRetrieving) {
                await chapterService.Load(Chapter);
                await OreService.Retrieve(Chapter, settings.CutoffTime);
            }
        }
    }
}
