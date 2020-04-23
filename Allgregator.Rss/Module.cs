using Allgregator.Aux.Common;
using Allgregator.Rss.Services;
using Allgregator.Rss.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Rss {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Given.MenuRegion, typeof(ChaptersView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterSingleton<ChapterService>();
        }
    }
}
