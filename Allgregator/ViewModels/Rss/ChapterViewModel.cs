using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly LinkRepository linkRepository;
        private readonly MinedRepository minedRepository;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            Chapter chapter,
            OreService oreService,
            Settings settings,
            LinkRepository linkRepository,
            MinedRepository minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            Chapter = chapter;
            OreService = oreService;
            this.settings = settings;
            this.linkRepository = linkRepository;
            this.minedRepository = minedRepository;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<RssChapterViews?>(async (view) => await ChangeView(view));
            OpenCommand = new DelegateCommand(OpenAll);
            MoveCommand = new DelegateCommand(MoveAll);
            UpdateCommand = new DelegateCommand(Update);
            CancelUpdateCommand = new DelegateCommand(OreService.CancelRetrieve);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(Closing);
            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe(CurrentChapterChanged);
        }

        public DelegateCommand<RssChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand CancelUpdateCommand { get; private set; }
        public OreService OreService { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            private set => SetProperty(ref isActive, value);
        }

        private RssChapterViews currentView = RssChapterViews.LinksView;//todo
        public RssChapterViews CurrentView {
            get => currentView;
            private set => SetProperty(ref currentView, value);
        }

        public void Activate() {
            eventAggregator.GetEvent<ChapterChangedEvent>().Publish(Chapter);
            settings.RssChapterId = Chapter.Id;
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
            await LoadMined();
            await LoadLinks();
        }

        private async void CurrentChapterChanged(Chapter chapter) {
            IsActive = chapter.Id == Chapter.Id;
            if (IsActive) {
                await CurrentViewChanged();
            }
            else {
                await Save();
            }
        }

        private void Closing(CancelEventArgs e) {
            var taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
            taskFactory.StartNew(Save).Unwrap().GetAwaiter().GetResult();
        }

        private async Task Save() {
            await SaveLinks();
            await SaveMined();
        }

        private void OpenAll() {
            if (IsActive) {
                var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
                var count = recos.Count;
                if (count > settings.RssMaxOpenTabs) {
                    dialogService.Show($"{count}?", OpenReal, null, 72d);
                }
                else {
                    OpenReal();
                }
            }
            else {
                Activate();
            }
        }

        private void OpenReal(object o = null) {
            var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
            if (recos != null) {
                foreach (var reco in recos) WindowUtilities.Run(reco.Uri.ToString());
            }
        }

        private void MoveAll() {
            if (Chapter?.Mined?.NewRecos != null && Chapter.Mined?.OldRecos != null) {
                Chapter.IsNeedToSaveMined = true;
                var cache = Chapter.Mined.NewRecos.Reverse();
                foreach (var reco in cache) {
                    Chapter.Mined.NewRecos.Remove(reco);
                    Chapter.Mined.OldRecos.Insert(0, reco);
                }
            }
        }

        private async void Update() {
            if (Chapter != null && !OreService.IsRetrieving) {
                await LoadLinks(true);
                await LoadMined(true);
                await OreService.Retrieve(Chapter);
            }
        }

        private async Task LoadLinks(bool force = false) {
            if (Chapter != null && Chapter.Links == null && (force || CurrentView == RssChapterViews.LinksView)) {
                IEnumerable<Link> chapters;
                try {
                    chapters = await linkRepository.Get(Chapter.Id);
                }
                catch (Exception e) {
                    /*//TODO Log*/
                    chapters = LinkRepository.CreateDefault();
                }

                Chapter.Links = new ObservableCollection<Link>(chapters);
            }
        }

        private async Task SaveLinks() {
            if (Chapter != null && Chapter.Links != null) {
                if (Chapter.IsNeedToSaveLinks) {
                    try {
                        await linkRepository.Save(Chapter.Id, Chapter.Links);
                        Chapter.IsNeedToSaveLinks = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
        }

        private async Task LoadMined(bool force = false) {
            if (Chapter != null && Chapter.Mined == null && (force || CurrentView != RssChapterViews.LinksView)) {
                try {
                    Chapter.Mined = await minedRepository.Get(Chapter.Id);
                }
                catch (Exception e) { /*//TODO Log*/ }

                if (chapter.Mined == null) {
                    chapter.Mined = new Mined();
                }
            }
        }

        private async Task SaveMined() {
            if (Chapter != null && Chapter.Mined != null) {
                if (Chapter.IsNeedToSaveMined) {
                    try {
                        await minedRepository.Save(Chapter.Id, Chapter.Mined);
                        Chapter.IsNeedToSaveMined = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
        }
    }
}
