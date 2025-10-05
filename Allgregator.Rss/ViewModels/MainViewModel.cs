using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Linq;

namespace Allgregator.Rss.ViewModels {
    internal class MainViewModel : BindableBase {
        private readonly Settings settings;
        private readonly DialogService dialogService;
        private readonly IRegionManager regionManager;
        private readonly Data data;

        public MainViewModel(
            IRegionManager regionManager,
            Data data,
            Settings settings,
            DialogService dialogService
            ) {
            this.regionManager = regionManager;
            this.data = data;
            this.settings = settings;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<ChapterViews?>(ChangeView);
            MoveCommand = new DelegateCommand(Move);
            BrowseCommand = new DelegateCommand(Browse);

            ChangeView(ChapterViews.NewsView);
        }
        public DelegateCommand<ChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand BrowseCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }

        private ChapterViews currentView;
        public ChapterViews CurrentView {
            get { return currentView; }
            private set { SetProperty(ref currentView, value); }
        }

        private void ChangeView(ChapterViews? chapterView) {
            CurrentView = chapterView ?? ChapterViews.NewsView;
            var viewName = CurrentView switch {
                ChapterViews.NewsView => typeof(NewsView).FullName,
                ChapterViews.OldsView => typeof(OldsView).FullName,
                ChapterViews.LinksView => typeof(LinksView).FullName,
                ChapterViews.SettingsView => typeof(SettingsView).FullName,
                _ => typeof(NewsView).FullName
            };
            var parameters = new NavigationParameters {
                { Given.DataParameter, data }
            };
            regionManager.RequestNavigate(Givenloc.SubmainRegion, viewName, parameters);
        }

        private void Browse() {
            var recos = CurrentView switch {
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
            var mined = data.Mined;
            if (mined != null && mined.NewRecos != null && mined.OldRecos != null) {
                mined.IsNeedToSave = true;
                foreach (var reco in mined.NewRecos.Reverse()) mined.OldRecos.Insert(0, reco);
                mined.NewRecos.Clear();
                mined.AcceptTime = mined.LastRetrieve;
            }
        }
    }
}
