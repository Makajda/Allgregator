using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase, IActiveAware {
        private readonly LinkRepository linksRepository;
        private readonly IRegionManager regionManager;

        public LinksViewModel(
            LinkRepository linksRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            this.linksRepository = linksRepository;
            this.regionManager = regionManager;

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe((chapter) => Chapter = chapter);
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveLinks);
        }

        public event EventHandler IsActiveChanged;

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set {
                SaveLinks();
                SetProperty(ref chapter, value);
                LoadLinks();
            }
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, OnIsActiveChanged);
        }

        private void OnIsActiveChanged() {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
            LoadLinks();
        }

        private void LoadLinks() {
            if (IsActive) {
                if (Chapter != null) {
                    if (Chapter.Links == null) {
                        Chapter.Links = new ObservableCollection<Link>(linksRepository.Get(Chapter.Id));
                    }
                }
            }
        }

        private void SaveLinks(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null) {
                //if (Chapter.Mined != null) {//TODO
                //    if (Chapter.Links) {
                //        linksRepository.Save(Chapter.Id, Chapter.Links);
                //        Chapter.Mined.IsNeedToSave = false;
                //    }
                //}
            }
        }
    }
}
