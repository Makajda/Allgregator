using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase, INavigationAware {
        public LinksViewModel() {
            //eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveSettings);
            //if (CurrentChapter != null) {
            //    if (CurrentChapter.Links == null) {
            //        CurrentChapter.Links = new ObservableCollection<Link>(linkRepository.GetLinks(CurrentChapter.Id));
            //    }
            //}
        }
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
        public void OnNavigatedTo(NavigationContext navigationContext) { }
    }
}
