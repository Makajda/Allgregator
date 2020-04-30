using Allgregator.Aux.Common;
using Allgregator.Fin.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Allgregator.Fin {
    public class Module : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.Register<IChapterView, ChapterView>(Given.SpecFin);
        }
    }
}
