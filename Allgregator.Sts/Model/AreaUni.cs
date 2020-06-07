using Allgregator.Aux.Common;
using System.Collections.Generic;

namespace Allgregator.Sts.Model {
    public class AreaUni {
        public string Name { get; set; }
        public IEnumerable<Pair<int, int>> Ranges { get; set; }
    }
}
