using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace Allgregator.Rss.Services {
    internal class ViewService {
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        private IRegionManager regionManagerLoc;
        public ViewService(
            IContainerExtension container,
            IRegionManager regionManager
            ) {
            this.container = container;
            this.regionManager = regionManager;
        }

        internal void ActivateMainView(int id) {
            const string name = "RssView";
            var region = regionManager.Regions[Aux.Common.Given.RegionMain];
            var viewName = GetName(name, id);
            var view = region.GetView(viewName);
            if (view == null) {
                view = container.Resolve<MainView>();
                regionManagerLoc = region.Add(view, viewName, true);
            }

            region.Activate(view);
        }

        internal void ManageMainViews(ChapterViews currentView, Data data) {
            var region = regionManagerLoc.Regions[Rss.Common.Given.RegionSubmain];
            var viewName = GetName(currentView.ToString(), data.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                var regionMain = regionManager.Regions[Aux.Common.Given.RegionMain];
                regionMain.Context = data;
                var type = currentView switch
                {
                    ChapterViews.NewsView => typeof(NewsView),
                    ChapterViews.OldsView => typeof(OldsView),
                    ChapterViews.LinksView => typeof(LinksView),
                    _ => typeof(SettingsView)
                };

                view = container.Resolve(type);
                region.Add(view, viewName);
            }

            region.Activate(view);
        }

        internal void RemoveMainViews(int id) {
            var region = regionManager.Regions[Aux.Common.Given.RegionMain];
            foreach (var view in Enum.GetNames(typeof(ChapterViews))) {//todo MainView
                RemoveView(region, view, id);
            }
        }

        internal void AddModuleViews(IEnumerable<Data> chapters) {
            var region = regionManager.Regions[Aux.Common.Given.RegionMenu];
            foreach (var chapter in chapters) {
                var view = container.Resolve<ChapterView>();
                view.SetIdAndName(chapter.Id, chapter.Name);//todo to ctor
                region.Add(view);
            }
        }

        private void RemoveView(IRegion region, string currentView, int id) {
            var view = region.GetView(GetName(currentView, id));
            if (view != null) region.Remove(view);
        }

        private string GetName(string view, int id) => $"{view}.{id}";
    }
}
