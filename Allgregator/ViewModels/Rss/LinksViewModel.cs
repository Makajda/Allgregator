using Allgregator.Common;
using Allgregator.Models.Rss;
using Prism.Events;
using Prism.Mvvm;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase {
        public LinksViewModel(
            IEventAggregator eventAggregator
            ) {
            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe((chapter) => Chapter = chapter);
        }


        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }
    }
}
