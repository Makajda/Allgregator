using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Serilog;
using System;
using System.Reflection;
using System.Windows;

namespace Allgregator {
    public partial class App : PrismApplication {
        protected override void OnInitialized() {
            base.OnInitialized();
            var settings = Container.Resolve<Settings>();
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(settings.CurrentChapterId);
        }

        protected override Window CreateShell() => WindowUtilities.CreateShell<MainWindow, MainWindowViewModel>(Container);

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();
            var settingsRepository = Container.Resolve<SettingsRepository>();
            containerRegistry.RegisterInstance(settingsRepository.GetOrDefault());
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
            moduleCatalog.AddModule<Rss.Module>("Rss");
            //moduleCatalog.AddModule<Fin.Module>("Fin");
        }
    }
}
