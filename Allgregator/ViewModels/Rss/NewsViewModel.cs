using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allgregator.ViewModels.Rss {
    public class NewsViewModel : BindableBase, INavigationAware {
        private readonly MinedRepository minedRepository;
        public NewsViewModel(
            MinedRepository minedRepository
            ) {
            this.minedRepository = minedRepository;
        }

        private Chapter chapter;
        public Chapter Chapter {
            get { return chapter; }
            set { SetProperty(ref chapter, value); }
        }

        private void ChapterChanged(Chapter chapter) {
            if (chapter != null) {
                if (chapter.Mined == null) {
                    chapter.Mined = minedRepository.GetMined(chapter.Id);
                }
            }

            Chapter = chapter;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
        public void OnNavigatedTo(NavigationContext navigationContext) { }
    }
}
