using Allgregator.Aux.Common;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Rss {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var viewService = containerProvider.Resolve<ViewService>();
            var chapterRepository = new ChapterRepository();
            var chapters = chapterRepository.GetOrDefault();
            viewService.AddModuleViews(chapters);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
        }
    }
}
