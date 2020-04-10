﻿using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly LinkRepository linkRepository;
        private readonly MinedRepository minedRepository;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            Chapter chapter,
            OreService oreService,
            Settings settings,
            LinkRepository linkRepository,
            MinedRepository minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            Chapter = chapter;
            OreService = oreService;
            this.settings = settings;
            this.linkRepository = linkRepository;
            this.minedRepository = minedRepository;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<RssChapterViews?>((view) => CurrentView = view ?? RssChapterViews.NewsView);
            OpenCommand = new DelegateCommand(OpenAll);
            MoveCommand = new DelegateCommand(MoveAll);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(async (cancelEventArgs) => await SaveMined(cancelEventArgs));
            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe(ChapterChanged);
        }

        public DelegateCommand<RssChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public OreService OreService { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            private set => SetProperty(ref isActive, value, () => { if (IsActive) SetView(); });
        }

        private RssChapterViews currentView;
        public RssChapterViews CurrentView {
            get => currentView;
            private set => SetProperty(ref currentView, value, SetView);
        }

        public async Task Activate() {
            eventAggregator.GetEvent<ChapterChangedEvent>().Publish(Chapter);
            settings.RssChapterId = Chapter.Id;
            await LoadMined();
        }

        private void SetView() {
            var region = regionManager.Regions[Given.MainRegion];
            var view = region.GetView(CurrentView.ToString());
            if (view != null) region.Activate(view);
        }

        private async void ChapterChanged(Chapter chapter) {
            IsActive = chapter.Id == Chapter.Id;
            if (!IsActive) {
                await SaveMined();
            }
        }

        private async void OpenAll() {
            if (IsActive) {
                var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
                var count = recos.Count;
                if (count > settings.RssMaxOpenTabs) {
                    dialogService.Show($"{count}?", OpenReal);
                }
                else {
                    OpenReal();
                }
            }
            else {
                await Activate();
            }
        }

        private void OpenReal() {
            var recos = CurrentView == RssChapterViews.NewsView ? Chapter?.Mined?.NewRecos : Chapter?.Mined?.OldRecos;
            if (recos != null) {
                foreach (var reco in recos) WindowUtilities.Run(reco.Uri.ToString());
            }
        }

        private void MoveAll() {
            if (Chapter?.Mined?.NewRecos != null && Chapter.Mined?.OldRecos != null) {
                Chapter.Mined.IsNeedToSave = true;
                var cache = Chapter.Mined.NewRecos.Reverse();
                foreach (var reco in cache) {
                    Chapter.Mined.NewRecos.Remove(reco);
                    Chapter.Mined.OldRecos.Insert(0, reco);
                }
            }
        }

        private async void Update() {
            if (Chapter != null) {
                if (Chapter.Links == null) {
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

                await LoadMined();
                await OreService.Retrieve(Chapter);
            }
        }

        private async Task LoadMined() {
            if (Chapter != null && Chapter.Mined == null) {
                try {
                    Chapter.Mined = await minedRepository.Get(Chapter.Id);
                }
                catch (Exception e) { /*//TODO Log*/ }

                if (chapter.Mined == null) {
                    chapter.Mined = new Mined();
                }
            }
        }

        private async Task SaveMined(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null && Chapter.Mined != null) {
                var mined = Chapter.Mined;
                if (mined.IsNeedToSave) {
                    try {
                        await minedRepository.Save(Chapter.Id, mined);
                        mined.IsNeedToSave = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
            //todo mayby save links or chapters
        }
    }
}
