using Allgregator.Aux.Common;
using Allgregator.Fin.Models;
using Prism.Mvvm;
using Prism.Regions;

namespace Allgregator.Fin.ViewModels {
    public class CurrencyViewModel : BindableBase {
        public CurrencyViewModel(
            IRegionManager regionManager
            ) {
            if (regionManager.Regions[Given.RegionMain].Context is Data data) {
                Data = data;
            }
        }
        public Data Data { get; private set; }
    }
}
