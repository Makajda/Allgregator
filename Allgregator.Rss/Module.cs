using Allgregator.Aux.Common;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace Allgregator.Rss {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            var region = regionManager.Regions[Given.MenuRegion];
            var chapterRepository = new ChapterRepository();
            var chapters = chapterRepository.GetOrDefault();
            foreach(var chapter in chapters) {
                var view = containerProvider.Resolve<ChapterView>();
                view.SetIdAndName(chapter.Id, chapter.Name);
                region.Add(view);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
        }
    }
}
