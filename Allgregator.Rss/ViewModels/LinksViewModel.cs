using Allgregator.Aux.Common;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class LinksViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;//todo filter rss
        private readonly DialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        public LinksViewModel(
            Data data,
            ChapterRepository chapterRepository,
            DialogService dialogService,
            DetectionService detectionService,
            IEventAggregator eventAggregator
            ) {
            Data = data;
            this.chapterRepository = chapterRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            AddCommand = new DelegateCommand(async () => await detectionService.SetAddress(Data.Linked));
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(link => detectionService.Selected(Data.Linked, link));
            ToChapterCommand = new DelegateCommand(ToChapter);
            FromChapterCommand = new DelegateCommand(FromChapter);
            DeleteChapterCommand = new DelegateCommand(DeleteChapter);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(SaveChapterName));
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }
        public DelegateCommand ToChapterCommand { get; private set; }
        public DelegateCommand FromChapterCommand { get; private set; }
        public DelegateCommand DeleteChapterCommand { get; private set; }
        public Data Data { get; private set; }

        private async void Move(Link link) {
            var chapters = await chapterRepository.GetOrDefault();
            dialogService.Show(chapters.Where(n => n.Id != Data.Id).Select(n => n.Name),
                name => {
                    var newChapter = chapters.FirstOrDefault(n => n.Name == name);
                    eventAggregator.GetEvent<LinkMovedEvent>().Publish((newChapter.Id, link));
                    if (Data.Linked?.Links != null) {
                        Data.Linked.IsNeedToSave = true;
                        Data.Linked.Links.Remove(link);
                    }
                });
        }

        private void Delete(Link link) {
            dialogService.Show($"{link.Name}?", DeleteReal, 20, true);

            void DeleteReal() {
                if (Data.Linked?.Links != null) {
                    Data.Linked.Links.Remove(link);
                    Data.Linked.IsNeedToSave = true;
                }
            }
        }

        private void ToChapter() {
            if (Data.Linked != null) {
                //todo savedName = Chapter.Name ?? string.Empty;
                Data.Linked.CurrentState = LinksStates.Chapter;
            }
        }

        private async void FromChapter() {
            if (Data.Linked != null) {
                Data.Linked.CurrentState = LinksStates.Normal;
                await SaveChapterName();
            }
        }

        private async Task SaveChapterName() {
            //todo if (savedName != null && Chapter.Name != savedName) {
            //    var chapters = await chapterRepository.GetOrDefault();
            //    var chapter = chapters.FirstOrDefault(n => n.Id == Chapter.Id);
            //    if (chapter != null) {
            //        chapter.Name = string.IsNullOrEmpty(Chapter.Name) ? null : Chapter.Name;
            //        try {
            //            await chapterRepository.Save(chapters);
            //            savedName = null;
            //        }
            //        catch (Exception e) {
            //            Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //        }
            //    }
            //}
        }

        private void DeleteChapter() {
            if (Data.Linked?.Links != null && Data.Linked.Links.Count > 0) {
                dialogService.Show($"{Data.Linked.Links.Count} addresses?", DeleteChapterReal, 20, true);
            }
            else {
                DeleteChapterReal();
            }

            void DeleteChapterReal() {
                //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Publish(id);
            }
        }
    }
}
