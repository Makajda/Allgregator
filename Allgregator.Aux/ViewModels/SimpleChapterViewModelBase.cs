using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public class SimpleChapterViewModelBase<TItem> : ChapterViewModelBase {
        private readonly string viewMain;
        private readonly string viewSettings;
        private readonly IRegionManager regionManager;
        private readonly RepositoryBase<MinedBase<TItem>> minedRepository;
        private bool isSettings;
        public SimpleChapterViewModelBase(
            string itemName,
            string moduleName,
            string viewMain,
            string viewSettings,
            RepositoryBase<MinedBase<TItem>> minedRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(settings, eventAggregator) {
            this.viewMain = viewMain;
            this.viewSettings = viewSettings;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
            minedRepository.SetNames(moduleName, $"{itemName}Mined");
            Data.Title = itemName;
            chapterId = $"{moduleName}{itemName}";
        }

        public DataBase<MinedBase<TItem>> Data { get; } = new DataBase<MinedBase<TItem>>();

        protected override async Task Activate() {
            ViewActivate();
            await LoadMined();
        }
        protected override async Task Deactivate() => await SaveMined();
        protected override void Run() {
            isSettings = !isSettings;
            ViewActivate();
        }
        protected override void WindowClosing(CancelEventArgs args) {
            base.WindowClosing(args);
            AsyncHelper.RunSync(async () => await SaveMined());
        }

        protected override Task Update() => Task.CompletedTask;

        protected virtual async Task LoadMined() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();
            }
        }

        private async Task SaveMined() {
            if (Data.Mined != null) {
                try {
                    await minedRepository.Save(Data.Mined);
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
        private void ViewActivate() {
            regionManager.RequestNavigate(Given.MainRegion, isSettings ? viewSettings : viewMain, new NavigationParameters {
                { Given.DataParameter, Data }
            });
        }
    }
}
