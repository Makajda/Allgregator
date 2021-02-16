using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System.Collections.Generic;

namespace Allgregator.Sts.Models {
    public class UnicodeArea : IName {
        public string Name { get; set; }
        public IEnumerable<Pair<int, int>> Ranges { get; set; }
    }
}
