using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Fin.Models;
using Allgregator.Fin.Repositories;
using Allgregator.Fin.Services;
using Allgregator.Fin.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Allgregator.Fin.ViewModels {
    internal class ChapterViewModel : BindableBase {
        private readonly Settings settings;
        private readonly FactoryService factoryService;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly MinedRepository minedRepository;

        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            FactoryService factoryService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            MinedRepository minedRepository
            ) {
            OreService = oreService;
            this.settings = settings;
            this.factoryService = factoryService;
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.minedRepository = minedRepository;

            OpenCommand = new DelegateCommand(Open);
            UpdateCommand = new DelegateCommand(Update);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(CurrentChapterChanged);
        }

        public OreService OreService { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        private Mined mined;
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }

        private async void CurrentChapterChanged(int chapterId) {
            IsActive = chapterId == Given.FinChapter;

            if (IsActive) {
                await LoadMined();
                var region = regionManager.Regions[Given.MainRegion];
                var viewName = typeof(CurrencyView).Name;
                var view = region.GetView(viewName);
                if (view == null) {
                    view = factoryService.Resolve<CurrencyView>();
                    if (view is FrameworkElement frameworkElement) {
                        frameworkElement.DataContext = this;
                    }

                    region.Add(view, viewName);
                }

                region.Activate(view);
            }
        }

        private void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = Given.FinChapter;
            AsyncHelper.RunSync(async () => await SaveMined());
        }

        private void Open() {
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Given.FinChapter);
        }

        private async void Update() {
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
