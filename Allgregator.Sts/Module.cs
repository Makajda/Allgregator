using Allgregator.Aux.Common;
using Allgregator.Sts.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Sts {
    public class Module : IModule {
        internal const string Name = "Sts";

        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(Given.MenuRegion, typeof(ChapterUniView).FullName);
            regionManager.RequestNavigate(Given.MenuRegion, typeof(ChapterClrView).FullName);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ChapterUniView>(typeof(ChapterUniView).FullName);
            containerRegistry.RegisterForNavigation<UnicodeView>(typeof(UnicodeView).FullName);
            containerRegistry.RegisterForNavigation<ChapterClrView>(typeof(ChapterClrView).FullName);
            containerRegistry.RegisterForNavigation<ColoresView>(typeof(ColoresView).FullName);
        }
    }
}
