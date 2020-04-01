using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services.Rss;
using Prism.Commands;
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

        public ChaptersViewModel(
            Settings settings,
            ChapterRepository chapterRepository,
            OreService oreService,
            IRegionManager regionManager
            ) {
            this.chapterRepository = chapterRepository;
            this.oreService = oreService;
            this.regionManager = regionManager;

            ActivateCommand = new DelegateCommand<Chapter>(Activate);
            NewsOldsCommand = new DelegateCommand(NewsOlds);
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

                CurrentChapter = currentChapter;
            }
            //TODO regionManager.RequestNavigate(Given.MainRegion, state);
        }

        public DelegateCommand<Chapter> ActivateCommand { get; private set; }
        public DelegateCommand NewsOldsCommand { get; private set; }
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

        private void Activate(Chapter chapter) {
            CurrentChapter = chapter;
        }

        private void NewsOlds() {
        }

        private void Links() {
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
