using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace Allgregator {
    public class MainWindowViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly IRegionManager regionManager;
        private readonly FactoryService factoryService;

        public MainWindowViewModel(
            ChapterRepository chapterRepository,
            IRegionManager regionManager,
            FactoryService factoryService,
            IEventAggregator eventAggregator
            ) {
            this.chapterRepository = chapterRepository;
            this.regionManager = regionManager;
            this.factoryService = factoryService;

            LoadCommand = new DelegateCommand(Load);
            //todo temp here
            SettingsCommand = new DelegateCommand(() => eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Given.SettingsChapter));
            //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(ChapterDeleted);
            //todo eventAggregator.GetEvent<ChapterAddedEvent>().Subscribe(ChapterAdded);
            //todo internal class ChapterDeletedEvent : PubSubEvent<int> { }
            //todo internal class ChapterAddedEvent : PubSubEvent<Chapter[]> { }
        }

        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }

        private string title = "Получение информации";
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private async void Load() {
            var chapters = await chapterRepository.GetOrDefault();
            if (chapters != null) {
                foreach (var chapter in chapters) {
                    var view = factoryService.Resolve<IChapterView>(chapter.Spec ?? Given.SpecRss);
                    view.SetChapter(chapter);
                    regionManager.Regions[Given.MenuRegion].Add(view);
                }
            }
        }

        private async void ChapterAdded(Chapter[] chapters) {
            foreach (var newChapter in chapters) {
                if (newChapter.Id == 0) {
                    //todo newChapter.Id = chapterRepository.GetNewId(Chapters.Select(n => n.Chapter));
                }

                //todo Chapters.Add(factoryService.Resolve<Chapter, ChapterViewModel>(newChapter));
            }

            //await Save();
        }

        private async void ChapterDeleted(int id) {
            //var chapter = Chapters.FirstOrDefault(n => n.Chapter.Id == id);
            //if (chapter != null) {
            //    Chapters.Remove(chapter);
            //    chapter.IsActive = false;
            //    await Save();

            //    viewsService.RemoveMainViews(chapter.Chapter);
            //}
        }

        //private async Task Save() {
        //    try {
        //        await chapterRepository.Save(Chapters.Select(n => n.Chapter));
        //    }
        //    catch (Exception e) {
        //        Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //    }
        //}
    }
}
