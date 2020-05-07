using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.Generic;

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
            var region = regionManager.Regions[Aux.Common.Given.RegionMain];
            var viewName = GetName(data.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                region.Context = data;
                view = container.Resolve<MainView>();
                var regionManagerLoc = region.Add(view, viewName, true);
            }

            region.Activate(view);
        }

        internal void ManageMainViews(ChapterViews currentView, Data data, string regionName) {
            var region = regionManager.Regions[regionName];
            var viewName = currentView.ToString();
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

        internal void RemoveMainView(int id) {
            var region = regionManager.Regions[Aux.Common.Given.RegionMain];
            var view = region.GetView(GetName(id));
            if (view != null) region.Remove(view);
        }

        internal void AddModuleViews(IEnumerable<Data> chapters) {
            var region = regionManager.Regions[Aux.Common.Given.RegionMenu];
            foreach (var chapter in chapters) {
                var view = container.Resolve<ChapterView>();
                view.SetIdAndName(chapter.Id, chapter.Name);//todo to ctor
                region.Add(view);
            }
        }

        private string GetName(int id) {
            const string name = "RssView";
            return $"{name}.{id}";
        }
    }
}
