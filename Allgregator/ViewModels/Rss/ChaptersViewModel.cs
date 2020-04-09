using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase, IActiveAware {
        private readonly ChapterRepository chapterRepository;
        private readonly int startChapterId;

        public ChaptersViewModel(
            ChapterRepository chapterRepository,
            Settings settings
            ) {
            this.chapterRepository = chapterRepository;
            startChapterId = settings.RssChapterId;

            IsActiveChanged += async (s, e) => await Load();
        }

        public event EventHandler IsActiveChanged;

        private ObservableCollection<ChapterViewModel> chapters;
        public ObservableCollection<ChapterViewModel> Chapters {
            get => chapters;
            set => SetProperty(ref chapters, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, () => IsActiveChanged?.Invoke(this, EventArgs.Empty));
        }

        private async Task Load() {
            if (IsActive && Chapters == null) {
                IEnumerable<Chapter> chapters = null;
                try {
                    chapters = await chapterRepository.Get();
                }
                catch (Exception) {
                    /*//TODO Log*/
                    chapters = ChapterRepository.CreateDefault();
                }

                if (chapters != null) {
                    var container = (App.Current as PrismApplication).Container;
                    Chapters = new ObservableCollection<ChapterViewModel>(chapters.Select(n => container.Resolve<ChapterViewModel>((typeof(Chapter), n))));

                    var currentChapter = Chapters.FirstOrDefault(n => n.Chapter.Id == startChapterId);
                    if (currentChapter == null) currentChapter = Chapters.FirstOrDefault();

                    if (currentChapter != null) currentChapter.Activate();
                }
            }
        }
    }
}
