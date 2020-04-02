using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.ComponentModel;

namespace Allgregator.ViewModels.Rss {
    public class RecosViewModel : BindableBase, IActiveAware {
        private readonly MinedRepository minedRepository;
        private readonly IRegionManager regionManager;

        public RecosViewModel(
            MinedRepository minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            this.minedRepository = minedRepository;
            this.regionManager = regionManager;

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveMined);
        }

        public event EventHandler IsActiveChanged;

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value, () => SaveMined());
        }

        private bool isNews;
        public bool IsNews {
            get => isNews;
            set => SetProperty(ref isNews, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, OnIsActiveChanged);
        }

        private void OnIsActiveChanged() {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);

            if (IsActive) {
                Chapter = regionManager.Regions[Given.MainRegion].Context as Chapter;
                if (Chapter != null) {
                    if (Chapter.Mined == null) {
                        Chapter.Mined = minedRepository.Get(Chapter.Id);
                    }
                }
            }
        }

        private void SaveMined(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null) {
                if (Chapter.Mined != null) {
                    if (Chapter.Mined.IsNeedToSave) {
                        minedRepository.Save(Chapter.Id, Chapter.Mined);
                        Chapter.Mined.IsNeedToSave = false;
                    }
                }
            }
        }
    }
}
