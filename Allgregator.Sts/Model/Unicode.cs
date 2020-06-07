using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System.Collections.Generic;

namespace Allgregator.Sts.Model {
    public class DataUnicode : DataBase<MinedUnicode> { }
    public class MinedUnicode : MinedBase<AreaUnicode> { }
    public class AreaUnicode : IName {
        public string Name { get; set; }
        public IEnumerable<Pair<int, int>> Ranges { get; set; }
    }
}
