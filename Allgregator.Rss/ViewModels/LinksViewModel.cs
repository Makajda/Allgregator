using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using System.Linq;

namespace Allgregator.Rss.ViewModels {
    internal class LinksViewModel : DataViewModelBase<Data> {
        private readonly ChapterRepository chapterRepository;
        private readonly DialogService dialogService;
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

            AddCommand = new DelegateCommand(async () => await detectionService.SetAddress(Data.Linked));
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(link => detectionService.Selected(Data.Linked, link));
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }

        private void Move(Link link) {
            var chapters = chapterRepository.GetOrDefault();
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
    }
}
