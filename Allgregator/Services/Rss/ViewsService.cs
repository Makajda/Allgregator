using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.ViewModels.Rss;
using Allgregator.Views.Rss;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace Allgregator.Services.Rss {
    public class ViewsService {
        private readonly IContainerProvider container;
        private readonly IRegionManager regionManager;
        public ViewsService(
            IContainerProvider container,
            IRegionManager regionManager
            ) {
            this.container = container;
            this.regionManager = regionManager;
        }


        public void ManageMainViews(RssChapterViews currentView, Chapter chapter) {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = $"{currentView}.{chapter.Id}";
            var view = region.GetView(viewName);
            if (view == null) {
                object viewModel;
                switch (currentView) {
                    case RssChapterViews.NewsView:
                        view = container.Resolve<RecosView>((typeof(bool), true));
                        viewModel = container.Resolve<RecosViewModel>((typeof(Chapter), chapter));
                        break;
                    case RssChapterViews.OldsView:
                        view = container.Resolve<RecosView>((typeof(bool), false));
                        viewModel = container.Resolve<RecosViewModel>((typeof(Chapter), chapter));
                        break;
                    case RssChapterViews.LinksView:
                    default:
                        view = container.Resolve<LinksView>();
                        viewModel = container.Resolve<LinksViewModel>((typeof(Chapter), chapter));
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
