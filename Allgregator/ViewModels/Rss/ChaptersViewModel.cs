using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase {
        private readonly IRegionManager regionManager;
        private readonly ChapterRepository chapterRepository;
        private readonly int startChapterId;

        public ChaptersViewModel(
            ChapterRepository chapterRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) {
            this.regionManager = regionManager;
            this.chapterRepository = chapterRepository;
            startChapterId = settings.RssChapterId;
            SettingsCommand = new DelegateCommand(OnSettingsCommand);

            eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(ChapterDeleted);
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
            //OnSettingsCommand();//todo
        }

        private async void ChapterDeleted(int id) {
            var chapter = Chapters.FirstOrDefault(n => n.Chapter.Id == id);
            if (chapter != null) {
                Chapters.Remove(chapter);
                chapter.Deactivate();
                try {
                    await chapterRepository.Save(Chapters.Select(n => n.Chapter));
                }
                catch (Exception e) {
                    /*//TODO Log*/
                }
            }
        }

        private void OnSettingsCommand() {
            foreach (var chapter in Chapters) chapter.Deactivate();
            var region = regionManager.Regions[Given.MainRegion];
            var viewControl = region.GetView(Given.SettingsView);
            if (viewControl != null) region.Activate(viewControl);
        }
    }
}
