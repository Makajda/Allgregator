using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Fin.Models;
using Allgregator.Fin.Repositories;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly MinedRepository minedRepository;
        private readonly IContainerExtension container;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IContainerExtension container,
            MinedRepository minedRepository
            ) : base(eventAggregator) {
            OreService = oreService;
            this.settings = settings;
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.container = container;
            this.minedRepository = minedRepository;
        }

        public OreService OreService { get; private set; }

        private Mined mined;
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }

        protected override async Task CurrentChapterChanged(int chapterId) {
            IsActive = chapterId == Given.FinChapter;

            if (IsActive) {
                await LoadMined();
                var region = regionManager.Regions[Given.MainRegion];
                var viewName = typeof(CurrencyView).Name;
                var view = region.GetView(viewName);
                if (view == null) {
                    view = container.Resolve<CurrencyView>();
                    region.Add(view, viewName);
                    if (view is FrameworkElement frameworkElement) {
                        frameworkElement.DataContext = this;
                    }
                }

                region.Activate(view);
            }
        }

        protected override void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = Given.FinChapter;
            AsyncHelper.RunSync(async () => await SaveMined());
        }

        protected override Task Open() {
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Given.FinChapter);
            return Task.CompletedTask;
        }

        protected override async Task Update() {
            if (OreService.IsRetrieving) {
                OreService.CancelRetrieve();
            }
            else {
                await LoadMined();
                settings.FinStartDate = new DateTimeOffset(2020, 3, 18, 0, 0, 0, TimeSpan.Zero);
                await OreService.Retrieve(Mined, settings.FinStartDate);
            }
        }

        private async Task LoadMined() {
            if (Mined == null) {
                Mined = await minedRepository.GetOrDefault();
            }
        }

        private async Task SaveMined() {
            try {
                await minedRepository.Save(Mined);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
