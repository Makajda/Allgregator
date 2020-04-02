using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Services.Rss;
using Allgregator.Views.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly OreService oreService;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public ChapterViewModel(
            Chapter chapter,
            OreService oreService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            Chapter = chapter;
            this.oreService = oreService;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            ActivateCommand = new DelegateCommand(Activate);
            NewsCommand = new DelegateCommand(() => SetVisibleNewsOldsLinks(false, true, true));
            OldsCommand = new DelegateCommand(() => SetVisibleNewsOldsLinks(true, false, true));
            LinksCommand = new DelegateCommand(() => SetVisibleNewsOldsLinks(true, true, false));
            OpenCommand = new DelegateCommand(Open);
            DeleteCommand = new DelegateCommand(Delete);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe((chapter) => IsActive = chapter.Id == Chapter.Id);
        }

        public DelegateCommand ActivateCommand { get; private set; }
        public DelegateCommand NewsCommand { get; private set; }
        public DelegateCommand OldsCommand { get; private set; }
        public DelegateCommand LinksCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
        }

        private bool isNewsVisible;
        public bool IsNewsVisible {
            get => isNewsVisible;
            set => SetProperty(ref isNewsVisible, value);
        }

        private bool isOldsVisible = true;
        public bool IsOldsVisible {
            get => isOldsVisible;
            set => SetProperty(ref isOldsVisible, value);
        }

        private bool isLinksVisible = true;
        public bool IsLinksVisible {
            get => isLinksVisible;
            set => SetProperty(ref isLinksVisible, value);
        }

        private void SetVisibleNewsOldsLinks(bool news, bool olds, bool links) {
            IsNewsVisible = news;
            IsOldsVisible = olds;
            IsLinksVisible = links;
            SetMainView();
        }

        private void SetMainView() {
            var type = IsNewsVisible ? (IsOldsVisible ? typeof(LinksView) : typeof(OldsView)) : typeof(NewsView);
            regionManager.RequestNavigate(Given.MainRegion, type.Name);
        }

        private void Activate() {
            eventAggregator.GetEvent<ChapterChangedEvent>().Publish(Chapter);
            SetMainView();
        }

        private void Open() {
            //TODO
        }

        private void Delete() {
            //TODO
        }

        private async void Update() {
            //TODO
            //await oreService.Retrieve(CurrentChapter);
        }
    }
}
