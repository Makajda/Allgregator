using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Allgregator.Spl.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Spl.ViewModels {
    internal class ButimeChapterViewModel : ChapterViewModelBase {
        private readonly IRegionManager regionManager;
        private readonly ZipRepositoryBase<MinedBase<Butime>> minedRepository;
        private bool isSettings;
        public ButimeChapterViewModel(
            IEventAggregator eventAggregator,
            Settings settings,
            ZipRepositoryBase<MinedBase<Butime>> minedRepository,
            IRegionManager regionManager
            ) : base(settings, eventAggregator) {
            this.minedRepository = minedRepository;
            this.regionManager = regionManager;
            chapterId = $"{Module.Name}Butimes";
        }
        public DataBase<MinedBase<Butime>> Data { get; } = new DataBase<MinedBase<Butime>> { Title = "Times" };
        protected override async Task Activate() {
            ViewActivate();
            await Load();
        }
        protected override async Task Deactivate() => await Save();
        protected override void Run() {
            isSettings = !isSettings;
            ViewActivate();
        }
        protected override void WindowClosing(CancelEventArgs args) {
            base.WindowClosing(args);
            AsyncHelper.RunSync(async () => await Save());
        }
        protected override Task Update() => Task.CompletedTask;

        private void ViewActivate() {
            var view = isSettings ? typeof(ButimeSettingsView).FullName : typeof(ButimeView).FullName;
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, view, parameters);
        }

        private async Task Load() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();
            }
        }

        private async Task Save() {
            if (Data.Mined != null) {
                try {
                    await minedRepository.Save(Data.Mined);
                }
                catch (Exception exception) {
                    Serilog.Log.Error(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
    }
}
