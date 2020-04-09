using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase, IActiveAware {
        private readonly LinkRepository linkRepository;
        private readonly IRegionManager regionManager;

        public LinksViewModel(
            LinkRepository linkRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            this.linkRepository = linkRepository;
            this.regionManager = regionManager;

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe(ChangeChapter);
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(async (cancelEventArgs) => await Save(cancelEventArgs));
            IsActiveChanged += async (s, e) => await Load();
        }

        public event EventHandler IsActiveChanged;

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, () => IsActiveChanged?.Invoke(this, EventArgs.Empty));
        }

        private async void ChangeChapter(Chapter chapter) {
            await Save();
            Chapter = chapter;
            await Load();
        }

        private async Task Load() {
            if (IsActive && Chapter != null && Chapter.Links == null) {
                IEnumerable<Link> chapters;
                try {
                    chapters = await linkRepository.Get(Chapter.Id);
                }
                catch (Exception e) {
                    /*//TODO Log*/
                    chapters = LinkRepository.CreateDefault();
                }

                Chapter.Links = new ObservableCollection<Link>(chapters);
            }
        }

        private async Task Save(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null && Chapter.Links != null) {
                try {
                    //todo needSave await linksRepository.Save(Chapter.Id, Chapter.Links);
                }
                catch (Exception e) { /*//TODO Log*/ }
            }
        }
    }
}

