using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
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

            OreCommand = new DelegateCommand(Ore);
            ActivateCommand = new DelegateCommand<Chapter>(Activate);

            var chapters = chapterRepository.GetChapters();
            if (chapters != null) {
                Chapters = new ObservableCollection<Chapter>(chapters);

                var currentChapter = Chapters.FirstOrDefault(n => n.Id == settings.RssChapterId);
                if (currentChapter == null) {
                    currentChapter = Chapters.FirstOrDefault();
                }

                CurrentChapter = currentChapter;
            }
        }

        public DelegateCommand OreCommand { get; private set; }
        public DelegateCommand<Chapter> ActivateCommand { get; private set; }

        public ObservableCollection<Chapter> Chapters { get; private set; }

        private Chapter currentChapter;
        public Chapter CurrentChapter {
            get => currentChapter;
            set => SetProperty(ref currentChapter, value);
        }

        private string chapterState = "1111";
        public string ChapterState {
            get => chapterState;
            set => SetProperty(ref chapterState, value);
        }

        private void Activate(Chapter chapter) {
            CurrentChapter = chapter;
        }

        private async void Ore() {
            await oreService.Retrieve(CurrentChapter);
            //regionManager.RequestNavigate(Given.MainRegion, state);
        }
    }
}
