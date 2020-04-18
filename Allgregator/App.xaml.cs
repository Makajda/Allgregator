using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Models.Rss;
using Allgregator.Services.Rss;
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
        protected override Window CreateShell() => WindowUtilities.GetMainWindow();

        protected override void OnInitialized() {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Given.MenuRegion, typeof(ChaptersView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterInstance<Settings>(WindowUtilities.GetSettings());
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

        public void ManageMainViews(RssChapterViews currentView, Chapter chapter) {
            var regionManager = Container.Resolve<IRegionManager>();
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = $"{currentView}.{chapter.Id}";
            var view = region.GetView(viewName);
            if (view == null) {
                object viewModel;
                switch (currentView) {
                    case RssChapterViews.NewsView:
                        view = Container.Resolve<RecosView>((typeof(bool), true));
                        viewModel = Container.Resolve<RecosViewModel>((typeof(Chapter), chapter));
                        break;
                    case RssChapterViews.OldsView:
                        view = Container.Resolve<RecosView>((typeof(bool), false));
                        viewModel = Container.Resolve<RecosViewModel>((typeof(Chapter), chapter));
                        break;
                    case RssChapterViews.LinksView:
                    default:
                        view = Container.Resolve<LinksView>();
                        viewModel = Container.Resolve<LinksViewModel>((typeof(Chapter), chapter));
                        break;
                }

                region.Add(view, viewName);
                if (view is FrameworkElement frameworkElement) {
                    frameworkElement.DataContext = viewModel;
                }
            }

            region.Activate(view);
        }
    }
}
