using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;

namespace Allgregator.ViewModels.Rss {
    public class CollectionsViewModel : BindableBase {
        CollectionRepository collectionRepository;
        LinkRepository linkRepository;
        MinedRepository minedRepository;
        OreService oreService;
        IRegionManager regionManager;

        public CollectionsViewModel (
            Settings settings,
            CollectionRepository collectionRepository,
            LinkRepository linkRepository,
            MinedRepository minedRepository,
            OreService oreService,
            IRegionManager regionManager
            ) {
            this.collectionRepository = collectionRepository;
            this.linkRepository = linkRepository;
            this.minedRepository = minedRepository;
            this.oreService = oreService;
            this.regionManager = regionManager;

            OreCommand = new DelegateCommand(Ore);
            StateCommand = new DelegateCommand<string>(SetState);
            Collections = new System.Collections.ObjectModel.ObservableCollection<Collection>(collectionRepository.GetCollections());//TODO null
            CurrentCollection = Collections.FirstOrDefault(n => n.Id == settings.RssCollectionId);
        }

        public DelegateCommand OreCommand { get; private set; }
        public DelegateCommand<string> StateCommand { get; private set; }

        private string state;
        public string State {
            get { return state; }
            set { SetProperty(ref state, value); }
        }

        private void SetState(string state) {
            regionManager.RequestNavigate(Given.MenuRegion, "CollectionsView");//TODO const
            State = state;
        }

        private async void Ore() {
            await oreService.Retrieve(CurrentCollection);
        }

        public System.Collections.ObjectModel.ObservableCollection<Collection> Collections { get; private set; }

        private Collection currentCollection;
        public Collection CurrentCollection {
            get { return currentCollection; }
            set { SetProperty(ref currentCollection, value, OnCurrentCollectionChanged); }
        }

        private void OnCurrentCollectionChanged() {
            if (CurrentCollection != null) {
                if (CurrentCollection.Links == null) {
                    CurrentCollection.Links = new System.Collections.ObjectModel.ObservableCollection<Link>(linkRepository.GetLinks(CurrentCollection.Id));
                }

                if (CurrentCollection.Mined == null) {
                    CurrentCollection.Mined = minedRepository.GetMined(CurrentCollection.Id);
                }
            }
        }
    }
}
