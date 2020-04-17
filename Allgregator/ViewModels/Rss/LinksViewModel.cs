using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly OpmlRepository opmlRepository;
        private readonly DialogService dialogService;
        private readonly DetectionService detectionService;
        private readonly IEventAggregator eventAggregator;
        string savedName;

        public LinksViewModel(
            ChapterRepository chapterRepository,
            OpmlRepository opmlRepository,
            DialogService dialogService,
            DetectionService detectionService,
            IEventAggregator eventAggregator
            ) {
            this.chapterRepository = chapterRepository;
            this.opmlRepository = opmlRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.detectionService = detectionService;

            AddCommand = new DelegateCommand(async () => await detectionService.SetAddress(Chapter.Linked));
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(link => detectionService.Selected(Chapter.Linked, link));
            ToChapterCommand = new DelegateCommand(ToChapter);
            FromChapterCommand = new DelegateCommand(FromChapter);
            ToOpmlCommand = new DelegateCommand(ToOpml);
            FromOpmlCommand = new DelegateCommand(FromOpml);
            DeleteAllCommand = new DelegateCommand(DeleteAll);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(SaveChapter));
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(chapter => Chapter = chapter);
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }
        public DelegateCommand ToChapterCommand { get; private set; }
        public DelegateCommand FromChapterCommand { get; private set; }
        public DelegateCommand ToOpmlCommand { get; private set; }
        public DelegateCommand FromOpmlCommand { get; private set; }
        public DelegateCommand DeleteAllCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private async void Move(Link link) {
            var chapters = await chapterRepository.GetOrDefault();
            dialogService.Show(chapters.Where(n => n.Id != Chapter.Id).Select(n => n.Name),
                name => {
                    var newChapter = chapters.FirstOrDefault(n => n.Name == name);
                    eventAggregator.GetEvent<LinkMovedEvent>().Publish((newChapter.Id, link));
                    Chapter.Linked.IsNeedToSave = true;
                    Chapter.Linked.Links.Remove(link);
                });
        }

        private void Delete(Link link) {
            dialogService.Show($"{link.Name}?", DeleteReal, 20, true);

            void DeleteReal() {
                Chapter.Linked.Links.Remove(link);
                Chapter.Linked.IsNeedToSave = true;
            }
        }

        private void ToChapter() {
            savedName = Chapter.Name;
            Chapter.Linked.CurrentState = RssLinksStates.Chapter;
        }

        private async void FromChapter() {
            Chapter.Linked.CurrentState = RssLinksStates.Normal;
            await SaveChapter();
        }

        private async Task SaveChapter() {
            if (savedName != null && Chapter.Name != savedName) {
                var chapters = await chapterRepository.GetOrDefault();
                var chapter = chapters.FirstOrDefault(n => n.Id == Chapter.Id);
                if (chapter != null) {
                    chapter.Name = Chapter.Name;
                    try {
                        await chapterRepository.Save(chapters);
                        savedName = default;
                    }
                    catch (Exception e) {
                        /*//TODO Log*/
                    }
                }
            }
        }

        private async void ToOpml() {
            try {
                await opmlRepository.Export();
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }

        private async void FromOpml() {
            try {
                var (chapters, links) = await opmlRepository.Import();
                var str = $"+ collections: {chapters},  RSS: {links}";
                dialogService.Show(str);
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }

        private void DeleteAll() {
            //todo
        }
    }
}
