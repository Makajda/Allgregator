﻿using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Allgregator.Sts.Repositories;
using Allgregator.Sts.Services;
using Allgregator.Sts.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.Sts.ViewModels {
    internal class ChapterClrViewModel : ChapterViewModelBase {
        private readonly IRegionManager regionManager;
        private readonly PaletteMinedRepository minedRepository;
        public ChapterClrViewModel(
            PaletteOreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            PaletteMinedRepository minedRepository,
            Settings settings
            ) : base(settings, eventAggregator) {
            OreService = oreService;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;
        }

        public DataPalette Data { get; } = new DataPalette();
        public PaletteOreService OreService { get; private set; }
        protected override string ChapterId => $"{Module.Name}Clr";
        protected override async Task Activate() {
            var view = typeof(ColoresView).FullName;
            var parameters = new NavigationParameters {
                { Given.DataParameter, Data }
            };
            regionManager.RequestNavigate(Given.MainRegion, view, parameters);
            await LoadMined();
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
                await OreService.Retrieve(Data.Mined, "");
            }
        }

        private async Task LoadMined() {
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
    }
}
