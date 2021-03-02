using System;

namespace Allgregator.Spl.Common {
    internal static class Givenloc {
        private const double NewDateInterval = 15d * 60d;
        internal static bool IsIncludedInTheInterval(DateTimeOffset date1, DateTimeOffset date2) {
            var result = Math.Abs((date1 - date2).TotalSeconds) < NewDateInterval;
            return result;
        }
    }
}
