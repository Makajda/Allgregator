using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Fin.Models;

namespace Allgregator.Fin.ViewModels {
    public class CurrencyViewModel : DataViewModelBase<Data> {
        public CurrencyViewModel(
            Settings settings
            ) {
            Settings = settings;
        }

        public Settings Settings { get; private set; }
    }
}
