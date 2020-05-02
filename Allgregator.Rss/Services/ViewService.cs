using Allgregator.Aux.Common;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;

namespace Allgregator.Rss.Services {
    internal class ViewService {
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        public ViewService(
            IContainerExtension container,
            IEventAggregator eventAggregator,
            IRegionManager regionManager
            ) {
            this.container = container;
            this.regionManager = regionManager;

            //todo temp here
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(OnSettingsCommand);
        }

        internal void ManageMainViews(ChapterViews currentView, Data data) {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = GetName(currentView.ToString(), data.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                region.Context = data;
                switch (currentView) {//todo expression
                    case ChapterViews.NewsView:
                        view = container.Resolve<NewsView>();
                        break;
                    case ChapterViews.OldsView:
                        view = container.Resolve<OldsView>();
                        break;
                    case ChapterViews.LinksView:
                    default:
                        view = container.Resolve<LinksView>();
                        break;
                }

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

        private void RemoveView(IRegion region, string currentView, int id) {
            var view = region.GetView(GetName(currentView, id));
            if (view != null) region.Remove(view);
        }

        private string GetName(string view, int id) => $"{view}.{id}";

        private void OnSettingsCommand(int chapterId) {
            if (chapterId == Given.SettingsChapter) {
                var region = regionManager.Regions[Given.MainRegion];
                var viewName = typeof(SettingsView).Name;
                var view = region.GetView(viewName);
                if (view == null) {
                    view = container.Resolve<SettingsView>();
                    region.Add(view, viewName);
                }

                region.Activate(view);
            }
        }
    }
}
