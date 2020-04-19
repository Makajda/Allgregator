using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Allgregator.Views.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase {
        private readonly FactoryService factoryService;
        private readonly ViewsService viewsService;
        private readonly IRegionManager regionManager;
        private readonly ChapterRepository chapterRepository;
        private readonly int startChapterId;

        public ChaptersViewModel(
            FactoryService factoryService,
            ViewsService viewsService,
            ChapterRepository chapterRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) {
            this.factoryService = factoryService;
            this.viewsService = viewsService;
            this.regionManager = regionManager;
            this.chapterRepository = chapterRepository;
            startChapterId = settings.RssChapterId;
            SettingsCommand = new DelegateCommand(OnSettingsCommand);

            eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(ChapterDeleted);
            eventAggregator.GetEvent<ChapterAddedEvent>().Subscribe(ChapterAdded);
        }

        public DelegateCommand SettingsCommand { get; private set; }

        private ObservableCollection<ChapterViewModel> chapters;
        public ObservableCollection<ChapterViewModel> Chapters {
            get => chapters;
            set => SetProperty(ref chapters, value);
        }

        public async Task Load() {
            var chapters = await chapterRepository.GetOrDefault();
            if (chapters != null) {
                Chapters = new ObservableCollection<ChapterViewModel>(
                    chapters.Select(n => factoryService.Resolve<Chapter, ChapterViewModel>(n)));

                var currentChapter = Chapters.FirstOrDefault(n => n.Chapter.Id == startChapterId);
                if (currentChapter == null) {
                    currentChapter = Chapters.FirstOrDefault();
                }

                currentChapter?.Activate();
            }
        }

        private async void ChapterAdded(Chapter[] chapters) {
            foreach (var newChapter in chapters) {
                if (newChapter.Id == 0) {
                    newChapter.Id = chapterRepository.GetNewId(Chapters.Select(n => n.Chapter));
                }

                Chapters.Add(factoryService.Resolve<Chapter, ChapterViewModel>(newChapter));
            }

            await Save();
        }

        private async void ChapterDeleted(int id) {
            var chapter = Chapters.FirstOrDefault(n => n.Chapter.Id == id);
            if (chapter != null) {
                Chapters.Remove(chapter);
                chapter.IsActive = false;
                await Save();

                viewsService.RemoveMainViews(chapter.Chapter);
            }
        }

        private async Task Save() {
            try {
                await chapterRepository.Save(Chapters.Select(n => n.Chapter));
            }
            catch (Exception e) {
                /*//TODO Log*/
            }
        }

        private void OnSettingsCommand() {
            foreach (var chapter in Chapters) chapter.IsActive = false;
            var region = regionManager.Regions[Given.MainRegion];
            var view = region.GetView(Given.SettingsView);
            if (view == null) {
                view = factoryService.Resolve<SettingsView>();
                region.Add(view, Given.SettingsView);
            }

            region.Activate(view);
        }
    }
}
