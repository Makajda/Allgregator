using Allgregator.Aux.Common;
using Allgregator.Fin.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Fin {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Given.MenuFinRegion, typeof(ChapterView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
        }
    }
}
