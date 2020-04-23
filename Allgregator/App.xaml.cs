using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
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
        protected override Window CreateShell() {
            return WindowUtilities.CreateShell<MainWindow, MainWindowViewModel>(Container);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();
            var settingsRepository = Container.Resolve<SettingsRepository>();
            containerRegistry.RegisterInstance(settingsRepository.GetOrDefault());
            containerRegistry.RegisterInstance(new FactoryService(Container));
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

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            var moduleType = typeof(Allgregator.Rss.Module);
            moduleCatalog.AddModule(
                new ModuleInfo() {
                    ModuleName = moduleType.Name,
                    ModuleType = moduleType.AssemblyQualifiedName
                });
        }
    }
}
