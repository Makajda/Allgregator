using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services.Rss;
using Allgregator.Views.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly OreService oreService;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public ChaptersViewModel(
            Settings settings,
            ChapterRepository chapterRepository,
            OreService oreService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            this.chapterRepository = chapterRepository;
            this.oreService = oreService;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            ActivateCommand = new DelegateCommand<Chapter>(Activate);
            NewsCommand = new DelegateCommand(News);
            OldsCommand = new DelegateCommand(Olds);
            LinksCommand = new DelegateCommand(Links);
            OpenCommand = new DelegateCommand(Open);
            DeleteCommand = new DelegateCommand(Delete);
            UpdateCommand = new DelegateCommand(Update);

            var chapters = chapterRepository.GetChapters();
            if (chapters != null) {
                Chapters = new ObservableCollection<Chapter>(chapters);

                var currentChapter = Chapters.FirstOrDefault(n => n.Id == settings.RssChapterId);
                if (currentChapter == null) {
                    currentChapter = Chapters.FirstOrDefault();
                }

                Activate(currentChapter);
            }
        }

        public DelegateCommand<Chapter> ActivateCommand { get; private set; }
        public DelegateCommand NewsCommand { get; private set; }
        public DelegateCommand OldsCommand { get; private set; }
        public DelegateCommand LinksCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        public ObservableCollection<Chapter> Chapters { get; private set; }

        private Chapter currentChapter;
        public Chapter CurrentChapter {
            get => currentChapter;
            set => SetProperty(ref currentChapter, value);
        }

        private bool isNewsVisible;
        public bool IsNewsVisible {
            get => isNewsVisible;
            set => SetProperty(ref isNewsVisible, value);
        }

        private bool isOldsVisible = true;
        public bool IsOldsVisible {
            get => isOldsVisible;
            set => SetProperty(ref isOldsVisible, value);
        }

        private bool isLinksVisible = true;
        public bool IsLinksVisible {
            get => isLinksVisible;
            set => SetProperty(ref isLinksVisible, value);
        }

        private void Activate(Chapter chapter) {
            CurrentChapter = chapter;
            eventAggregator.GetEvent<ChapterChangedEvent>().Publish(chapter);
        }

        private void News() {
            IsNewsVisible = false;
            IsOldsVisible = true;
            IsLinksVisible = true;
            regionManager.RequestNavigate(Given.MainRegion, typeof(NewsView).Name);
        }

        private void Olds() {
            IsNewsVisible = true;
            IsOldsVisible = false;
            IsLinksVisible = true;
            regionManager.RequestNavigate(Given.MainRegion, typeof(OldsView).Name);
        }

        private void Links() {
            IsNewsVisible = true;
            IsOldsVisible = true;
            IsLinksVisible = false;
            regionManager.RequestNavigate(Given.MainRegion, typeof(LinksView).Name);
        }

        private void Open() {
        }

        private void Delete() {
        }

        private async void Update() {
            await oreService.Retrieve(CurrentChapter);
        }
    }
}
