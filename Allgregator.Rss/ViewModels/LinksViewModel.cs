using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;

namespace Allgregator.Rss.ViewModels {
    internal class LinksViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly DialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        public LinksViewModel(
            IRegionManager regionManager,
            ChapterRepository chapterRepository,
            DialogService dialogService,
            DetectionService detectionService,
            IEventAggregator eventAggregator
            ) {
            if (regionManager.Regions[Given.MainRegion].Context is Data data) {
                Data = data;
            }

            this.chapterRepository = chapterRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            AddCommand = new DelegateCommand(async () => await detectionService.SetAddress(Data.Linked));
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(link => detectionService.Selected(Data.Linked, link));
            //todo SettingsCommand = new DelegateCommand(() => viewService.ManageMainViews(ChapterViews.SettingsView, Data));
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }
        public Data Data { get; private set; }

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
