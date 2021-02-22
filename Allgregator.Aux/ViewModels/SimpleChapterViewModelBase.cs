using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public abstract class SimpleChapterViewModelBase<TMined> : ChapterViewModelBase where TMined : IWatchSave, new() {
        private readonly string viewMain;
        private readonly string viewSettings;
        private readonly IRegionManager regionManager;
        private readonly RepositoryBase<TMined> repository;
        private bool isSettings;
        public SimpleChapterViewModelBase(
            string itemName,
            string moduleName,
            string viewMain,
            string viewSettings,
            RepositoryBase<TMined> repository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            Settings settings
            ) : base(eventAggregator, settings) {
            this.viewMain = viewMain;
            this.viewSettings = viewSettings;
            this.regionManager = regionManager;
            this.repository = repository;
            repository.SetNames(moduleName, $"{itemName}Data");
            Data.Title = itemName;
            chapterId = $"{moduleName}{itemName}";
        }

        public DataBase<TMined> Data { get; } = new DataBase<TMined>();

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

        protected virtual async Task LoadMined() {
            if (Data.Mined == null) {
                Data.Mined = await repository.GetOrDefault();
            }
        }

        private async Task SaveMined() {
            if (Data.Mined != null) {
                try {
                    await repository.Save(Data.Mined);
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
        private void ViewActivate() {
            Data.IsSettings = isSettings;
            if (!isSettings || viewSettings != null)
                regionManager.RequestNavigate(Given.MainRegion, isSettings ? viewSettings : viewMain, new NavigationParameters { { Given.DataParameter, Data } });
        }
    }
}
