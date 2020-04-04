using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly LinkRepository linkRepository;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public ChapterViewModel(
            Chapter chapter,
            LinkRepository linkRepository,
            OreService oreService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            Chapter = chapter;
            OreService = oreService;
            this.linkRepository = linkRepository;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            ViewsCommand = new DelegateCommand<RssChapterViews?>((view) => CurrentView = view ?? RssChapterViews.NewsView);
            OpenCommand = new DelegateCommand(Open);
            DeleteCommand = new DelegateCommand(Delete);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe((chapter) => IsActive = chapter.Id == Chapter.Id);
        }

        public DelegateCommand<RssChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public OreService OreService { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            private set => SetProperty(ref isActive, value, () => { if (IsActive) SetView(); });
        }

        private RssChapterViews currentView;
        public RssChapterViews CurrentView {
            get => currentView;
            private set => SetProperty(ref currentView, value, SetView);
        }

        public void Activate() {
            eventAggregator.GetEvent<ChapterChangedEvent>().Publish(Chapter);
        }

        private void SetView() {
            var region = regionManager.Regions[Given.MainRegion];
            var view = region.GetView(CurrentView.ToString());
            if (view != null)
                region.Activate(view);
        }

        private void Open() {
            if (IsActive) {
                //TODO open all
            }
            else {
                Activate();
            }
        }

        private void Delete() {
            //TODO
        }

        private async void Update() {
            if (Chapter != null) {
                if (Chapter.Links == null) {
                    Chapter.Links = new ObservableCollection<Link>(linkRepository.Get(Chapter.Id));
                }

                await OreService.Retrieve(Chapter);
                if (Chapter.Mined != null) {
                    Chapter.Mined.IsNeedToSave = true;
                }
            }
        }
    }
}
