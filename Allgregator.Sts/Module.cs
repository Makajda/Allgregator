using Allgregator.Aux.Common;
using Allgregator.Sts.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Allgregator.Sts {
    public class Module : IModule {
        public const string Name = "Sts";

        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(Given.MenuRegion, typeof(Views.Cbr.ChapterView).FullName);
            regionManager.RequestNavigate(Given.MenuRegion, typeof(UnicodeChapterView).FullName);
            regionManager.RequestNavigate(Given.MenuRegion, typeof(PaletteChapterView).FullName);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<Views.Cbr.ChapterView>(typeof(Views.Cbr.ChapterView).FullName);
            containerRegistry.RegisterForNavigation<Views.Cbr.CurrencyView>(typeof(Views.Cbr.CurrencyView).FullName);
            containerRegistry.RegisterForNavigation<Views.Cbr.SettingsView>(typeof(Views.Cbr.SettingsView).FullName);
            containerRegistry.RegisterForNavigation<UnicodeChapterView>(typeof(UnicodeChapterView).FullName);
            containerRegistry.RegisterForNavigation<UnicodeView>(typeof(UnicodeView).FullName);
            containerRegistry.RegisterForNavigation<PaletteChapterView>(typeof(PaletteChapterView).FullName);
            containerRegistry.RegisterForNavigation<PaletteView>(typeof(PaletteView).FullName);
        }
    }
}
