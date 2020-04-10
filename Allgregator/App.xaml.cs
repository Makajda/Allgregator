using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Repositories.Rss;
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
        protected override Window CreateShell() => WindowUtilities.GetMainWindow();

        protected override void OnInitialized() {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            var region = regionManager.Regions[Given.MainRegion];
            region.Add(Container.Resolve<RecosView>((typeof(bool), true)), RssChapterViews.NewsView.ToString());
            region.Add(Container.Resolve<RecosView>((typeof(bool), false)), RssChapterViews.OldsView.ToString());
            region.Add(Container.Resolve<LinksView>(), RssChapterViews.LinksView.ToString());
            regionManager.RegisterViewWithRegion(Given.MenuRegion, typeof(ChaptersView));//last for subscriptions of ChapterChangedEvent in Activate()
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            var settingsRepository = Container.Resolve<SettingsRepository>();
            Settings settings;
            try {
                settings = settingsRepository.Get();
            }
            catch (Exception) {
                settings = new Settings();
            }

            containerRegistry.RegisterInstance<Settings>(settings);
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
