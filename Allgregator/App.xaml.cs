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
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            var settings = new SettingsRepository().GetSettings();//TODO try
            containerRegistry.RegisterInstance(settings);
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }
    }
}
