using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Fin.Models;
using Allgregator.Fin.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.ComponentModel;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly IEventAggregator eventAggregator;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator
            ) {
            OreService = oreService;
            this.settings = settings;
            this.eventAggregator = eventAggregator;

            OpenCommand = new DelegateCommand(Open);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(CurrentChapterChanged);
        }

        public OreService OreService { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        private void CurrentChapterChanged(int chapterId) {
            IsActive = chapterId == Given.FinChapter;
        }

        private void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = Given.FinChapter;
            //todo AsyncHelper.RunSync(async () => await chapterService.Save(Chapter));
        }

        private void Open() {
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Given.FinChapter);
        }

        private async void Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                var mined = new Mined();//todo from repository
                await OreService.Retrieve(mined);
            }
        }
    }
}
