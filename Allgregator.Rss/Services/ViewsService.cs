using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.ViewModels;
using Allgregator.Rss.Views;
using Prism.Regions;
using System;
using System.Windows;

namespace Allgregator.Rss.Services {
    internal class ViewsService {
        private readonly FactoryService factoryService;
        private readonly IRegionManager regionManager;
        public ViewsService(
            FactoryService factoryService,
            IRegionManager regionManager
            ) {
            this.factoryService = factoryService;
            this.regionManager = regionManager;
        }

        internal void ManageMainViews(ChapterViews currentView, Chapter chapter) {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = GetName(currentView.ToString(), chapter.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                object viewModel;
                switch (currentView) {
                    case ChapterViews.NewsView:
                        view = factoryService.Resolve<bool, RecosView>(true);
                        viewModel = factoryService.Resolve<Chapter, RecosViewModel>(chapter);
                        break;
                    case ChapterViews.OldsView:
                        view = factoryService.Resolve<bool, RecosView>(false);
                        viewModel = factoryService.Resolve<Chapter, RecosViewModel>(chapter);
                        break;
                    case ChapterViews.LinksView:
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

        internal void RemoveMainViews(Chapter chapter) {
            var region = regionManager.Regions[Given.MainRegion];
            foreach (var view in Enum.GetNames(typeof(ChapterViews))) {
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
