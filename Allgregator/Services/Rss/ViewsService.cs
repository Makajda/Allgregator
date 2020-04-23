using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.ViewModels.Rss;
using Allgregator.Views.Rss;
using Prism.Regions;
using System;
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
            var viewName = GetName(currentView.ToString(), chapter.Id);
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

                if (view is FrameworkElement frameworkElement) {
                    frameworkElement.DataContext = viewModel;
                }

                region.Add(view, viewName);
            }

            region.Activate(view);
        }

        public void RemoveMainViews(Chapter chapter) {
            var region = regionManager.Regions[Given.MainRegion];
            foreach (var view in Enum.GetNames(typeof(RssChapterViews))) {
                RemoveView(region, view, chapter.Id);
            }
        }

        private void RemoveView(IRegion region, string currentView, int id) {
            var view = region.GetView(GetName(currentView, id));
            if (view != null) region.Remove(view);
        }

        private string GetName(string view, int id) => $"{view}.{id}";
    }
}
