﻿using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly IEventAggregator eventAggregator;
        private readonly ChapterService chapterService;
        private readonly ViewService viewService;
        private readonly DialogService dialogService;

        public ChapterViewModel(
            OreService oreService,
            ChapterService chapterService,
            ViewService viewService,
            Settings settings,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            OreService = oreService;
            this.chapterService = chapterService;
            this.viewService = viewService;
            this.settings = settings;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<ChapterViews?>(async (view) => await ChangeView(view));
            OpenCommand = new DelegateCommand(Open);
            MoveCommand = new DelegateCommand(Move);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(CurrentChapterChanged);
            eventAggregator.GetEvent<LinkMovedEvent>().Subscribe(async n => await chapterService.LinkMoved(Data, n));
            //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(id => chapterService.DeleteFiles(id));
        }

        public DelegateCommand<ChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public OreService OreService { get; private set; }
        public Data Data { get; } = new Data();

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        private ChapterViews currentView;// = ChapterViews.LinksView;//todo
        public ChapterViews CurrentView {
            get => currentView;
            private set => SetProperty(ref currentView, value);
        }

        internal void SetIdAndName(int id, string name) {
            Data.Id = id;
            Data.Name = name;
        }

        private async Task ChangeView(ChapterViews? view) {
            if (IsActive) {
                CurrentView = CurrentView = view ?? ChapterViews.NewsView;
                await CurrentViewChanged();
            }
        }

        private async Task CurrentViewChanged() {
            viewService.ManageMainViews(CurrentView, Data);
            await chapterService.Load(Data, CurrentView == ChapterViews.LinksView);
        }

        private async void CurrentChapterChanged(int chapterId) {
            if (IsActive) {
                await chapterService.Save(Data);
            }

            IsActive = chapterId == Data.Id;

            if (IsActive) {
                await CurrentViewChanged();
            }
        }

        private void Open() {
            if (IsActive) {
                if (CurrentView != ChapterViews.LinksView) {
                    var recos = CurrentView == ChapterViews.NewsView ? Data.Mined?.NewRecos : Data.Mined?.OldRecos;
                    if (recos != null) {
                        var count = recos.Count;
                        if (count > settings.RssMaxOpenTabs) {
                            dialogService.Show($"{count}?", OpenReal, 72d);
                        }
                        else {
                            OpenReal();
                        }
                    }
                }
            }
            else {
                eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Data.Id);
            }
        }

        private void OpenReal() {
            var mined = Data.Mined;
            if (mined != null) {
                if (CurrentView == ChapterViews.NewsView) {
                    if (mined.NewRecos != null && mined.OldRecos != null) {
                        foreach (var reco in mined.NewRecos.Reverse()) {
                            WindowUtilities.Run(reco.Uri);
                            mined.OldRecos.Insert(0, reco);
                        }
                    }

                    mined.NewRecos.Clear();
                    mined.AcceptTime = mined.LastRetrieve;
                }
                else {
                    if (mined.OldRecos != null) {
                        foreach (var reco in mined.OldRecos) WindowUtilities.Run(reco.Uri);
                    }
                }
            }
        }

        private void Move() {
            var mined = Data.Mined;
            if (mined != null && mined.NewRecos != null && mined.OldRecos != null) {
                mined.IsNeedToSave = true;
                foreach (var reco in mined.NewRecos.Reverse()) mined.OldRecos.Insert(0, reco);
                mined.NewRecos.Clear();
                mined.AcceptTime = mined.LastRetrieve;
            }
        }

        private async void Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await chapterService.Load(Data);
                await OreService.Retrieve(Data, settings.RssCutoffTime);
            }
        }

        private void WindowClosing(CancelEventArgs e) {
            if (IsActive) settings.CurrentChapterId = Data.Id;
            AsyncHelper.RunSync(async () => await chapterService.Save(Data));
        }
    }
}
