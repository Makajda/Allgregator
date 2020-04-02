using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.ComponentModel;
using System.Linq;

namespace Allgregator.ViewModels.Rss {
    public class NewsViewModel : BindableBase, INavigationAware {
        private readonly MinedRepository minedRepository;

        public NewsViewModel(
            MinedRepository minedRepository,
            IEventAggregator eventAggregator
            ) {
            this.minedRepository = minedRepository;
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveMined);
        }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) {
            SaveMined();
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            Chapter = navigationContext?.Parameters.FirstOrDefault().Value as Chapter;
            if (Chapter != null) {
                if (Chapter.Mined == null) {
                    Chapter.Mined = minedRepository.Get(Chapter.Id);
                }
            }
        }

        private void SaveMined(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null) {
                if (Chapter.Mined != null) {
                    if (Chapter.Mined.IsNeedToSave) {
                        minedRepository.Save(Chapter.Id, Chapter.Mined);
                    }
                }
            }
        }
    }
}
