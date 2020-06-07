using Allgregator.Aux.Models;

namespace Allgregator.Sts.Model {
    public class PaletteColor : IName {
        public string Name { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}
