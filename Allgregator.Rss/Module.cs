using Allgregator.Aux.Common;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Allgregator.Rss {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.Register<IChapterView, ChapterView>(Given.SpecRss);
        }
    }
}
