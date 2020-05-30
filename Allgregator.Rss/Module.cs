using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Allgregator.Rss {
    public class Module : IModule {
        internal const string Name = "Rss";

        public void OnInitialized(IContainerProvider containerProvider) {
            var viewService = containerProvider.Resolve<ViewService>();
            var chapterRepository = new ChapterRepository();
            var chapters = chapterRepository.GetOrDefault();
            viewService.AddMenuViews(chapters);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ChapterView>(typeof(ChapterView).FullName);
            containerRegistry.RegisterForNavigation<NewsView>(typeof(NewsView).FullName);
            containerRegistry.RegisterForNavigation<OldsView>(typeof(OldsView).FullName);
            containerRegistry.RegisterForNavigation<LinksView>(typeof(LinksView).FullName);
            containerRegistry.RegisterForNavigation<SettingsView>(typeof(SettingsView).FullName);
        }
    }
}
