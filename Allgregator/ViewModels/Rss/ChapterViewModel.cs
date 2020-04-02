using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services.Rss;
using Allgregator.Views.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly OreService oreService;
        private readonly LinkRepository linkRepository;
        private readonly IRegionManager regionManager;

        public ChapterViewModel(
            Chapter chapter,
            LinkRepository linkRepository,
            OreService oreService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            Chapter = chapter;
            this.oreService = oreService;
            this.linkRepository = linkRepository;
            this.regionManager = regionManager;

            ActivateCommand = new DelegateCommand(() => eventAggregator.GetEvent<ChapterChangedEvent>().Publish(Chapter));
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
            set { SetProperty(ref isActive, value, () => { if (isActive) SetMainView(); }); }
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
            var parameters = new NavigationParameters();
            parameters.Add(null, Chapter);
            regionManager.RequestNavigate(Given.MainRegion, type.Name, parameters);
        }

        private void Open() {
            //TODO
        }

        private void Delete() {
            //TODO
        }

        private async void Update() {
            if (Chapter.Links == null) {
                Chapter.Links = new ObservableCollection<Link>(linkRepository.Get(Chapter.Id));
            }

            await oreService.Retrieve(Chapter);
            Chapter.Mined.IsNeedToSave = true;
        }
    }
}
