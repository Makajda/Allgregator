using Allgregator.Fin.Models;
using Prism.Mvvm;

namespace Allgregator.Fin.ViewModels {
    public class CurrencyViewModel : BindableBase {
        public CurrencyViewModel(
            Mined mined
            ) {
            Mined = mined;
        }

        public Mined Mined { get; private set; }
    }
}
