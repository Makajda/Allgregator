using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly int startChapterId;
        private string savedName;
        private DateTimeOffset savedLastRetrieve, savedAcceptTime;

        public ChaptersViewModel(
            ChapterRepository chapterRepository,
            Settings settings
            ) {
            this.chapterRepository = chapterRepository;
            startChapterId = settings.RssChapterId;
         
            SettingsCommand = new DelegateCommand(OnSettings);
        }

        public DelegateCommand SettingsCommand { get; private set; }

        private ObservableCollection<ChapterViewModel> chapters;
        public ObservableCollection<ChapterViewModel> Chapters {
            get => chapters;
            set => SetProperty(ref chapters, value);
        }

        public async Task Load() {
            if (Chapters == null) {
                var chapters = await chapterRepository.GetOrDefault();
                if (chapters != null) {
                    var container = (App.Current as PrismApplication).Container;
                    Chapters = new ObservableCollection<ChapterViewModel>(chapters.Select(n => container.Resolve<ChapterViewModel>((typeof(Chapter), n))));

                    var currentChapter = Chapters.FirstOrDefault(n => n.Chapter.Id == startChapterId);
                    if (currentChapter == null) {
                        currentChapter = Chapters.FirstOrDefault();
                    }

                    currentChapter?.Activate();
                }
            }
        }

        private void OnSettings() {
        }

        private void ToChapter() {
            //savedName = Chapter.Name;
            //savedLastRetrieve = Chapter.Mined.LastRetrieve;
            //savedAcceptTime = Chapter.Mined.AcceptTime;
            //Chapter.Linked.CurrentState = RssLinksStates.Chapter;
        }

        private void FromChapter() {
            //Chapter.Linked.CurrentState = RssLinksStates.Normal;
            //if (Chapter.Name != savedName) {
            //    savedName = default;
            //    var chapters = await chapterRepository.GetOrDefault();
            //    var chapter = chapters.FirstOrDefault(n => n.Id == Chapter.Id);
            //    if (chapter != null) {
            //        chapter.Name = Chapter.Name;
            //        try {
            //            await chapterRepository.Save(chapters);
            //        }
            //        catch (Exception e) {
            //            /*//TODO Log*/
            //        }
            //    }
            //}

            //Chapter.Mined.IsNeedToSave = Chapter.Mined.LastRetrieve != savedLastRetrieve || Chapter.Mined.AcceptTime != savedAcceptTime;
            //savedLastRetrieve = default;
            //savedAcceptTime = default;
        }
    }
}
