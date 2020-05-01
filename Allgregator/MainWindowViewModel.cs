using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace Allgregator {
    public class MainWindowViewModel : BindableBase {
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly FactoryService factoryService;

        public MainWindowViewModel(
            IRegionManager regionManager,
            FactoryService factoryService,
            IEventAggregator eventAggregator
            ) {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.factoryService = factoryService;

            //todo temp here
            SettingsCommand = new DelegateCommand(() => eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Given.SettingsChapter));
            //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(ChapterDeleted);
            //todo eventAggregator.GetEvent<ChapterAddedEvent>().Subscribe(ChapterAdded);
            //todo internal class ChapterDeletedEvent : PubSubEvent<int> { }
            //todo internal class ChapterAddedEvent : PubSubEvent<Chapter[]> { }
        }
        public DelegateCommand SettingsCommand { get; private set; }

        private string title = "Получение информации";
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        //private async void ChapterAdded(Chapter[] chapters) {
        //    foreach (var newChapter in chapters) {
        //        if (newChapter.Id == 0) {
        //            newChapter.Id = chapterRepository.GetNewId(Chapters.Select(n => n.Chapter));
        //        }

        //        Chapters.Add(factoryService.Resolve<Chapter, ChapterViewModel>(newChapter));
        //    }

        //    await Save();
        //}

        //private async void ChapterDeleted(int id) {
        //    var chapter = Chapters.FirstOrDefault(n => n.Chapter.Id == id);
        //    if (chapter != null) {
        //        Chapters.Remove(chapter);
        //        chapter.IsActive = false;
        //        await Save();

        //        viewsService.RemoveMainViews(chapter.Chapter);
        //    }
        //}

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
