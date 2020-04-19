using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.ViewModels.Rss;
using Allgregator.Views.Rss;
using Prism.Regions;
using System.Windows;

namespace Allgregator.Services.Rss {
    public class ViewsService {
        private readonly FactoryService factoryService;
        private readonly IRegionManager regionManager;
        public ViewsService(
            FactoryService factoryService,
            IRegionManager regionManager
            ) {
            this.factoryService = factoryService;
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
                        view = factoryService.Resolve<bool, RecosView>(true);
                        viewModel = factoryService.Resolve<Chapter, RecosViewModel>(chapter);
                        break;
                    case RssChapterViews.OldsView:
                        view = factoryService.Resolve<bool, RecosView>(false);
                        viewModel = factoryService.Resolve<Chapter, RecosViewModel>(chapter);
                        break;
                    case RssChapterViews.LinksView:
                    default:
                        view = factoryService.Resolve<LinksView>();
                        viewModel = factoryService.Resolve<Chapter, LinksViewModel>(chapter);
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
