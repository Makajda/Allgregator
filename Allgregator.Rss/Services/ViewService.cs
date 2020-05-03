using Allgregator.Aux.Common;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Regions;
using System;

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

        internal void ManageMainViews(ChapterViews currentView, Data data) {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = GetName(currentView.ToString(), data.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                region.Context = data;
                var type = currentView switch
                {
                    ChapterViews.NewsView => typeof(NewsView),
                    ChapterViews.OldsView => typeof(OldsView),
                    ChapterViews.LinksView => typeof(LinksView),
                    _ => typeof(NewsView)
                };

                view = container.Resolve(type);
                region.Add(view, viewName);
            }

            region.Activate(view);
        }

        internal void RemoveMainViews(int id) {//todo
            var region = regionManager.Regions[Given.MainRegion];
            foreach (var view in Enum.GetNames(typeof(ChapterViews))) {
                RemoveView(region, view, id);
            }
        }

        internal void Settings() {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = typeof(SettingView).Name;
            var view = region.GetView(viewName);
            if (view == null) {
                view = container.Resolve<SettingView>();
                region.Add(view, viewName);
            }

            region.Activate(view);
        }

        private void RemoveView(IRegion region, string currentView, int id) {
            var view = region.GetView(GetName(currentView, id));
            if (view != null) region.Remove(view);
        }

        private string GetName(string view, int id) => $"{view}.{id}";
    }
}
