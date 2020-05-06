using Allgregator.Aux.Common;
using Allgregator.Sts.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Sts {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            var region = regionManager.Regions[Given.MenuRegion];
            region.Add(containerProvider.Resolve<ChapterView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
        }
    }
}
