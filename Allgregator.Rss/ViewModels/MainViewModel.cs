using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class MainViewModel : BindableBase {
        private readonly Settings settings;
        private readonly RepoService repoService;
        private readonly ViewService viewService;
        private readonly DialogService dialogService;
        private ChapterViews currentView;// = ChapterViews.SettingsView;//todo

        public MainViewModel(
            RepoService repoService,
            ViewService viewService,
            Settings settings,
            DialogService dialogService
            ) {
            this.repoService = repoService;
            this.viewService = viewService;
            this.settings = settings;
            this.dialogService = dialogService;

            ViewsCommand = new DelegateCommand<ChapterViews?>(async (view) => await ChangeView(view));
            MoveCommand = new DelegateCommand(Move);
        }
        public DelegateCommand<ChapterViews?> ViewsCommand { get; private set; }
        public DelegateCommand MoveCommand { get; private set; }
        public Data Data { get; } = new Data();

        private async Task ChangeView(ChapterViews? view) {
            currentView = currentView = view ?? ChapterViews.NewsView;
            viewService.ManageMainViews(currentView, Data);
            await repoService.Load(Data, currentView == ChapterViews.LinksView);
        }
        private void Run() {
            var recos = currentView switch
            {
                ChapterViews.NewsView => Data.Mined?.NewRecos,
                ChapterViews.OldsView => Data.Mined?.OldRecos,
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
            var mined = Data.Mined;
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

        private async Task CurrentViewChanged() {
            viewService.ManageMainViews(currentView, Data);
            await repoService.Load(Data, currentView == ChapterViews.LinksView);
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
    }
}
