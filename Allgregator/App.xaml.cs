using Allgregator.Aux.Common;
using Allgregator.Aux.Repository;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Serilog;
using System;
using System.Reflection;
using System.Windows;

namespace Allgregator {
    public partial class App : PrismApplication {
        protected override Window CreateShell() => WindowUtilities.CreateShell<MainWindow, MainWindowViewModel>(Container);

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();
            var settingsRepository = Container.Resolve<SettingsRepository>();
            containerRegistry.RegisterInstance(settingsRepository.GetOrDefault());
        }

        protected override void ConfigureViewModelLocator() {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => {
                var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<Rss.Module>(Rss.Module.Name);
            moduleCatalog.AddModule<Spl.Module>(Spl.Module.Name);
            moduleCatalog.AddModule<Sts.Module>(Sts.Module.Name);
        }
    }
}
