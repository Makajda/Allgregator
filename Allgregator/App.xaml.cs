using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Repositories.Rss;
using Allgregator.ViewModels;
using Allgregator.Views;
using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;

namespace Allgregator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication {
        protected override Window CreateShell() {
            var mainWindow = Container.Resolve<MainWindow>();
            var settings = Container.Resolve<Settings>();

            WindowUtilities.SetWindowBoundsAndState(mainWindow, settings.MainWindowBounds, settings.MainWindowState);

            mainWindow.Closing += (s, e) => {
                var settings = Container.Resolve<Settings>();
                var settingsRepository = Container.Resolve<SettingsRepository>();
                settings.MainWindowBounds = mainWindow.RestoreBounds;
                settings.MainWindowState = mainWindow.WindowState;
                settingsRepository.Save(settings);
            };

            return mainWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            var settings = new SettingsRepository().GetSettings();//TODO try
            containerRegistry.RegisterInstance(settings);
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }
    }
}
