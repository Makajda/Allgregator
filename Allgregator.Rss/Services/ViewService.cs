using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.ViewModels;
using Allgregator.Rss.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows;

namespace Allgregator.Rss.Services {
    internal class ViewService {
        private readonly FactoryService factoryService;
        private readonly IRegionManager regionManager;
        public ViewService(
            FactoryService factoryService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager
            ) {
            this.factoryService = factoryService;
            this.regionManager = regionManager;

            //todo temp here
            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(OnSettingsCommand);
        }

        internal void ManageMainViews(ChapterViews currentView, Data data) {
            var region = regionManager.Regions[Given.MainRegion];
            var viewName = GetName(currentView.ToString(), data.Id);
            var view = region.GetView(viewName);
            if (view == null) {
                object viewModel;
                switch (currentView) {
                    case ChapterViews.NewsView:
                        view = factoryService.Resolve<bool, RecosView>(true);
                        viewModel = factoryService.Resolve<Data, RecosViewModel>(data);
                        break;
                    case ChapterViews.OldsView:
                        view = factoryService.Resolve<bool, RecosView>(false);
                        viewModel = factoryService.Resolve<Data, RecosViewModel>(data);
                        break;
                    case ChapterViews.LinksView:
                    default:
                        view = factoryService.Resolve<LinksView>();
                        viewModel = factoryService.Resolve<Data, LinksViewModel>(data);
                        break;
                }

                if (view is FrameworkElement frameworkElement) {
                    frameworkElement.DataContext = viewModel;
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
                    view = factoryService.Resolve<SettingsView>();
                    region.Add(view, viewName);
                }

                region.Activate(view);
            }
        }
    }
}
