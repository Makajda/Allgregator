using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.ViewModels;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.Generic;
using System.Windows;

namespace Allgregator.Rss.Services {
    internal class ViewService {
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        public ViewService(
            IContainerExtension container,
            IRegionManager regionManager
            ) {
            this.container = container;
            this.regionManager = regionManager;
        }

        internal void ActivateMainView(Data data) {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = GetName(data.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                view = container.Resolve<MainView>();
                var regionManagerLoc = region.Add(view, viewName, true);
                var viewModel = container.Resolve<MainViewModel>((typeof(IRegionManager), regionManagerLoc), (typeof(Data), data));
                if (view is FrameworkElement element) {
                    element.DataContext = viewModel;
                }
            }

            region.Activate(view);
        }

        internal void RemoveMainView(int id) {
            var region = regionManager.Regions[Given.MainRegion];
            var view = region.GetView(GetName(id));
            if (view != null) region.Remove(view);
        }

        internal void AddMenuViews(IEnumerable<Data> chapters) {
            var region = regionManager.Regions[Given.MenuRegion];
            foreach (var chapter in chapters) {
                var parameters = new NavigationParameters {
                    { Common.Givenloc.ChapterIdParameter, chapter.Id },
                    { Common.Givenloc.ChapterNameParameter, chapter.Title }
                };
                region.RequestNavigate(typeof(ChapterView).FullName, parameters);
            }
        }

        internal void RemoveMenuView(int id) {
            var region = regionManager.Regions[Given.MenuRegion];
            var views = region.Views;
            foreach(var view in views) {
                if (view is FrameworkElement element) {
                    if(element.DataContext is ChapterViewModel viewModel) {
                        if(viewModel.Data.Id == id) {
                            region.Remove(view);
                        }
                    }
                }
            }
        }

        private string GetName(int id) {
            const string name = "RssView";
            return $"{name}.{id}";
        }
    }
}
