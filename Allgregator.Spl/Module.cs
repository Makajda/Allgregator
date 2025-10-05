using Allgregator.Aux.Common;
using Allgregator.Spl.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;

namespace Allgregator.Spl {
    public class Module : IModule {
        public const string Name = "Spl";

        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(Given.MenuRegion, typeof(ButimeChapterView).FullName);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ButimeChapterView>(typeof(ButimeChapterView).FullName);
            containerRegistry.RegisterForNavigation<ButimeSettingsView>(typeof(ButimeSettingsView).FullName);
            containerRegistry.RegisterForNavigation<ButimeView>(typeof(ButimeView).FullName);
        }
    }
}
