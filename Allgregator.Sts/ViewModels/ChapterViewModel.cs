using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Repositories;
using Allgregator.Sts.Services;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Sts.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly IRegionManager regionManager;
        private readonly MinedRepository minedRepository;
        private readonly OreService oreService;
        private readonly Settings settings;
        public ChapterViewModel(
            OreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            MinedRepository minedRepository,
            Settings settings
            ) : base(eventAggregator) {
            this.oreService = oreService;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
            this.settings = settings;
        }

        public Data Data { get; } = new Data();
        protected override int ChapterId => Given.StsChapter;
        public override OreServiceBase OreService => oreService;
        public override IEnumerable<Error> Errors => Data.Mined?.Errors;
        protected override async Task Activate() {
            await LoadMined();
            RaisePropertyChanged(nameof(Errors));
            var view = typeof(UnicodeView).FullName;
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, view, parameters);
        }
        protected override async Task Deactivate() => await SaveMined();
        protected override void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = ChapterId;
            AsyncHelper.RunSync(async () => await SaveMined());
        }
        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await LoadMined();
                //await OreService.Retrieve(Data.Mined);
                RaisePropertyChanged(nameof(Errors));
            }
        }

        private async Task LoadMined() {
            if (Data.Mined == null) {
                Data.Mined = await minedRepository.GetOrDefault();
            }
        }

        private async Task SaveMined() {
            if (Data.Mined != null && Data.Mined.IsNeedToSave) {
                try {
                    await minedRepository.Save(Data.Mined);
                    Data.Mined.IsNeedToSave = false;
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
    }
}
