using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Aux.Common {
    public abstract class ChapterViewModelBase : BindableBase {
        protected readonly IEventAggregator eventAggregator;
        public ChapterViewModelBase(
            IEventAggregator eventAggregator
            ) {
            this.eventAggregator = eventAggregator;

            OpenCommand = new DelegateCommand(Open);
            UpdateCommand = new DelegateCommand(async () => await Update());

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(CurrentChapterChanged);
        }

        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        protected abstract int ChapterId { get; }
        protected abstract Task Activate();
        protected abstract Task Update();
        protected abstract void WindowClosing(CancelEventArgs args);
        protected abstract Task Deactivate();
        protected virtual void Run() { }

        private async void CurrentChapterChanged(int chapterId) {
            var savedIsActive = IsActive;
            IsActive = ChapterId == chapterId;
            if (savedIsActive) {
                await Deactivate();
            }

            if (IsActive) {
                await Activate();
            }
        }

        private void Open() {
            if (IsActive) {
                Run();
            }
            else {
                eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(ChapterId);
            }
        }
    }
}