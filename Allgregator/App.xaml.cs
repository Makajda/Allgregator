using Allgregator.Common;
using Allgregator.Models;
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
using System;
using System.Reflection;
using System.Windows;

namespace Allgregator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication {
        protected override Window CreateShell() => WindowUtilities.GetMainWindow(Container);

        protected override void OnInitialized() {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Given.MenuRegion, typeof(ChaptersView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            var settingsRepository = Container.Resolve<SettingsRepository>();
            var settings = settingsRepository.GetOrDefault();
            containerRegistry.RegisterInstance<Settings>(settings);
            containerRegistry.RegisterSingleton<FactoryService>();
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
