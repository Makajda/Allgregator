using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;

        public ChaptersViewModel(
            Settings settings,
            ChapterRepository chapterRepository
            ) {
            this.chapterRepository = chapterRepository;

            var chapters = chapterRepository.Get();
            if (chapters != null) {
                var container = (App.Current as PrismApplication).Container;
                Chapters = new ObservableCollection<ChapterViewModel>(chapters.Select(n => container.Resolve<ChapterViewModel>((typeof(Chapter), n))));

                var currentChapter = Chapters.FirstOrDefault(n => n.Chapter.Id == settings.RssChapterId);
                if (currentChapter == null) currentChapter = Chapters.FirstOrDefault();

                if (currentChapter != null) currentChapter.Activate();
            }
        }

        public ObservableCollection<ChapterViewModel> Chapters { get; private set; }
    }
}
