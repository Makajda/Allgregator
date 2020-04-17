using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Allgregator.ViewModels.Rss {
    public class ChaptersViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly int startChapterId;

        public ChaptersViewModel(
            ChapterRepository chapterRepository,
            IEventAggregator eventAggregator,
            Settings settings
            ) {
            this.chapterRepository = chapterRepository;
            startChapterId = settings.RssChapterId;
            SettingsCommand = new DelegateCommand(OnSettings);

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

        private void OnSettings() {
            //todo
        }
        //public DelegateCommand ToOpmlCommand { get; private set; }
        //public DelegateCommand FromOpmlCommand { get; private set; }

        //            <Button Content = "to opml" Command="{Binding ToOpmlCommand}"/>
        //            <Button Content = "from opml" Command="{Binding FromOpmlCommand}"/>
        //ToOpmlCommand = new DelegateCommand(ToOpml);
        //FromOpmlCommand = new DelegateCommand(FromOpml);
        //private async void ToOpml() {
        //    try {
        //        await opmlRepository.Export();
        //    }
        //    catch (Exception exception) {
        //        dialogService.Show(exception.Message);
        //    }
        //}

        //private async void FromOpml() {
        //    try {
        //        var (chapters, links) = await opmlRepository.Import();
        //        var str = $"+ collections: {chapters},  RSS: {links}";
        //        dialogService.Show(str);
        //    }
        //    catch (Exception exception) {
        //        dialogService.Show(exception.Message);
        //    }
        //}
    }
}
