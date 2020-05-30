using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Allgregator.Sts.ViewModels {
    public class UnicodeViewModel : DataViewModelBase<Data> {
        private IEnumerable<Symbol> symbols;
        public IEnumerable<Symbol> Symbols {
            get => symbols;
            set => SetProperty(ref symbols, value);
        }
    }
}
