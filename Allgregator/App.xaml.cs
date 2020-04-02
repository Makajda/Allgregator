using Allgregator.Common;
using Allgregator.Repositories.Rss;
using Allgregator.ViewModels;
using Allgregator.ViewModels.Rss;
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

            var region = regionManager.Regions[Given.MainRegion];
            var recosView = Container.Resolve<RecosView>();
            var recosViewModel = Container.Resolve<RecosViewModel>();
            recosViewModel.IsNews = true;
            recosView.DataContext = recosViewModel;
            region.Add(recosView, RssChapterViews.NewsView.ToString());
            region.Add(Container.Resolve<RecosView>(), RssChapterViews.OldsView.ToString());
            region.Add(Container.Resolve<LinksView>(), RssChapterViews.LinksView.ToString());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            var settingsRepository = Container.Resolve<SettingsRepository>();
            containerRegistry.RegisterInstance(settingsRepository.Get());
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
