using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace Allgregator.Rss {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var viewService = containerProvider.Resolve<ViewService>();
            var chapterRepository = new ChapterRepository();
            var chapters = chapterRepository.GetOrDefault();
            viewService.AddMenuView(chapters);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
        }
    }
}
