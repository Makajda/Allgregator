using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public abstract class ChapterViewModelBase : BindableBase, INavigationAware {
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

        private bool isUpdatable = true;
        public bool IsUpdatable {
            get { return isUpdatable; }
            set { SetProperty(ref isUpdatable, value); }
        }

        protected abstract int ChapterId { get; }
        protected abstract Task Activate();
        protected abstract Task Update();
        protected abstract void WindowClosing(CancelEventArgs args);
        protected abstract Task Deactivate();
        public virtual OreServiceBase OreService { get; }
        public virtual IEnumerable<Error> Errors { get; }
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

        public virtual void OnNavigatedTo(NavigationContext navigationContext) { }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => false;
        public virtual void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}