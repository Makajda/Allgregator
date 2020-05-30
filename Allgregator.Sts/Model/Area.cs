using System.Collections.Generic;

namespace Allgregator.Sts.Model {
    public class Area {
        public string Name { get; set; }
        public List<(int Begin, int End)> Ranges { get; } = new List<(int Begin, int End)>();
    }
}
