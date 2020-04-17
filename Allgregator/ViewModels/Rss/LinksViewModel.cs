using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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
            DeleteAllCommand = new DelegateCommand(DeleteAll);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(SaveChapterName));
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(chapter => Chapter = chapter);
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }
        public DelegateCommand ToChapterCommand { get; private set; }
        public DelegateCommand FromChapterCommand { get; private set; }
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
            await SaveChapterName();
        }

        private async Task SaveChapterName() {
            if (savedName != null && Chapter.Name != savedName) {
                var chapters = await chapterRepository.GetOrDefault();
                var chapter = chapters.FirstOrDefault(n => n.Id == Chapter.Id);
                if (chapter != null) {
                    chapter.Name = Chapter.Name;
                    var saved = await SaveChapters(chapters);
                    if (saved) {
                        savedName = default;
                    }
                }
            }
        }

        private void DeleteAll() {
            dialogService.Show($"{Chapter.Linked.Links.Count} links?", DeleteReal, 20, true);

            void DeleteReal() {
                eventAggregator.GetEvent<ChapterDeletedEvent>().Publish(Chapter.Id);
            }
        }

        private async Task<bool> SaveChapters(IEnumerable<Chapter> chapters) {
            try {
                await chapterRepository.Save(chapters);
                return true;
            }
            catch (Exception e) {
                /*//TODO Log*/
            }

            return false;
        }
    }
}
