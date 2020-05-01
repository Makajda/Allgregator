using Allgregator.Aux.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Allgregator {
    public class MainWindowViewModel : BindableBase {
        public MainWindowViewModel(
            IEventAggregator eventAggregator
            ) {
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
