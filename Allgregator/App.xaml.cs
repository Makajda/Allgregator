using Allgregator.Common;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Allgregator.ViewModels;
using Allgregator.Views;
using Allgregator.Views.Rss;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.Reflection;
using System.Windows;

namespace Allgregator {
    public partial class App : PrismApplication {
        protected override Window CreateShell() => WindowUtilities.GetMainWindow(Container);

        protected override void OnInitialized() {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Given.MenuRegion, typeof(ChaptersView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();
            var settingsRepository = Container.Resolve<SettingsRepository>();
            containerRegistry.RegisterInstance(settingsRepository.GetOrDefault());
            containerRegistry.RegisterInstance(new FactoryService(Container));
            containerRegistry.RegisterSingleton<ChapterService>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }

        protected override void ConfigureServiceLocator() {
            base.ConfigureServiceLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => {
                var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
    }
}
