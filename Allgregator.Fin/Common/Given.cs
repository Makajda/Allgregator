using System.Collections.Generic;
using System.Windows.Media;

namespace Allgregator.Fin.Common {
    static class Given {
        public static readonly Dictionary<string, Brush> CurrencyData = new Dictionary<string, Brush>() {
            { "USD", Brushes.Blue },
            { "EUR",Brushes.White },
            { "GBP",Brushes.Red },
            { "CHF",Brushes.Brown },
            { "CNY",Brushes.MistyRose },
            { "UAH",Brushes.Yellow }
        };
    }
}
