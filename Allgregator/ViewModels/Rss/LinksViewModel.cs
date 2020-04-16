using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Linq;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly DialogService dialogService;
        private readonly DetectionService detectionService;
        private readonly IEventAggregator eventAggregator;
        public LinksViewModel(
            ChapterRepository chapterRepository,
            DialogService dialogService,
            DetectionService detectionService,
            IEventAggregator eventAggregator
            ) {
            this.chapterRepository = chapterRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.detectionService = detectionService;

            ToChapterCommand = new DelegateCommand(() => Chapter.Linked.CurrentState = RssLinksStates.Chapter);
            BackCommand = new DelegateCommand(() => Chapter.Linked.CurrentState = RssLinksStates.Normal);
            AddCommand = new DelegateCommand(async () => await detectionService.SetAddress(Chapter.Linked));
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(link => detectionService.Selected(Chapter.Linked, link));

            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(chapter => Chapter = chapter);
        }

        public DelegateCommand ToChapterCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }

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
    }
}
