using Allgregator.Aux.Common;
using Allgregator.Fin.Common;
using Allgregator.Fin.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Fin {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(Given.MenuRegion, typeof(ChapterView).FullName);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ChapterView>(typeof(ChapterView).FullName);
            containerRegistry.RegisterForNavigation<CurrencyView>(typeof(CurrencyView).FullName);
            containerRegistry.RegisterForNavigation<SettingsView>(typeof(SettingsView).FullName);
        }
    }
}
