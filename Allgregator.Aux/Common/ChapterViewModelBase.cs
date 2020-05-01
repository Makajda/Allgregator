using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Aux.Common {
    public abstract class ChapterViewModelBase : BindableBase {
        public ChapterViewModelBase(
            IEventAggregator eventAggregator
            ) {

            OpenCommand = new DelegateCommand(async () => await Open());
            UpdateCommand = new DelegateCommand(async () => await Update());

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(async id => await CurrentChapterChanged(id));
        }

        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        protected abstract Task CurrentChapterChanged(int chapterId);
        protected abstract void WindowClosing(CancelEventArgs args);
        protected abstract Task Open();
        protected abstract Task Update();
    }
}
