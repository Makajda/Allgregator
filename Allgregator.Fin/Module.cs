using Allgregator.Aux.Common;
using Allgregator.Fin.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Fin {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            var region = regionManager.Regions[Given.RegionMenu];
            region.Add(containerProvider.Resolve<ChapterView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
        }
    }
}
