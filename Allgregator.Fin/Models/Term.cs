using System;
using System.Collections.Generic;

namespace Allgregator.Fin.Models {
    public class Term {
        public DateTimeOffset Date { get; set; }
        public Dictionary<string, decimal> Values { get; set; }
    }
}
