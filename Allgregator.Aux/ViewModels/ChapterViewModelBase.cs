using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public abstract class ChapterViewModelBase : BindableBase, INavigationAware {
        protected readonly Settings settings;
        protected readonly IEventAggregator eventAggregator;
        public ChapterViewModelBase(
            Settings settings,
            IEventAggregator eventAggregator
            ) {
            this.settings = settings;
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
            protected set => SetProperty(ref isActive, value);

        }

        protected abstract string ChapterId { get; }
        protected abstract Task Activate();
        protected abstract Task Update();
        protected abstract void WindowClosing(CancelEventArgs args);
        protected abstract Task Deactivate();
        protected virtual void Run() { }

        private async void CurrentChapterChanged(string chapterId) {
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

        public virtual void OnNavigatedTo(NavigationContext navigationContext) {
            if (settings.CurrentChapterId == ChapterId) {
                IsActive = true;
                Activate();
            }
        }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => false;
        public virtual void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}