using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Allgregator.Spl.Models {
    public class Butask {
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Butime> Butimes { get; set; }
        public int Value { get; set; }
    }

    public class Butime {
        public DateTimeOffset Date { get; set; }
        public int Value { get; set; }
    }
}
