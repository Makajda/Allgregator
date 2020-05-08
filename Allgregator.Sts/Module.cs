using Allgregator.Aux.Common;
using Allgregator.Sts.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Sts {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(Given.MenuRegion, typeof(ChapterView).FullName);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ChapterView>(typeof(ChapterView).FullName);
            containerRegistry.RegisterForNavigation<UnicodeView>(typeof(UnicodeView).FullName);
        }
    }
}
