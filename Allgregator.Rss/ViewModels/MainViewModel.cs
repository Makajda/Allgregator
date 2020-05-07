using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Linq;

namespace Allgregator.Rss.ViewModels {
    internal class MainViewModel : BindableBase, INavigationAware {
        private readonly Settings settings;
        private readonly ViewService viewService;
        private readonly DialogService dialogService;
        private ChapterViews currentView;// = ChapterViews.SettingsView;//todo
        private Data data;

        public MainViewModel(
            RepoService repoService,
            ViewService viewService,
            IRegionManager regionManager,
            Settings settings,
            DialogService dialogService
            ) {
            this.viewService = viewService;
            this.settings = settings;
            this.dialogService = dialogService;

            //if (regionManager.Regions[Aux.Common.Given.RegionMain].Context is Data data) {
            //    this.data = data;
            //}

            ViewsCommand = new DelegateCommand<ChapterViews?>(ChangeView);
            MoveCommand = new DelegateCommand(Move);
        }
        public DelegateCommand<ChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }

        private void ChangeView(ChapterViews? view) {
            currentView = view ?? ChapterViews.NewsView;
            viewService.ManageMainViews(currentView, data, RegionName);
        }

        private void Run() {
            var recos = currentView switch
            {
                ChapterViews.NewsView => data.Mined?.NewRecos,
                ChapterViews.OldsView => data.Mined?.OldRecos,
                _ => null
            };

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

        private void OpenReal() {
            var mined = data.Mined;
            if (mined != null) {
                if (currentView == ChapterViews.NewsView) {
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
            var mined = data.Mined;
            if (mined != null && mined.NewRecos != null && mined.OldRecos != null) {
                mined.IsNeedToSave = true;
                foreach (var reco in mined.NewRecos.Reverse()) mined.OldRecos.Insert(0, reco);
                mined.NewRecos.Clear();
                mined.AcceptTime = mined.LastRetrieve;
            }
        }

        private string regionName;
        public string RegionName {
            get { return regionName; }
            set { SetProperty(ref regionName, value); }
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            var p = navigationContext.Parameters.GetValue<Data>("Data");
            data = p;
            RegionName = p.Id.ToString();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            var p = navigationContext.Parameters.GetValue<Data>("Data");
            return data.Id == p.Id;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) {
        }
    }
}
