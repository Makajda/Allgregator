using System;
using System.Collections.Generic;

namespace Allgregator.Sts.Cbr.Models {
    public class Term {
        public DateTimeOffset Date { get; set; }
        public Dictionary<string, decimal> Values { get; set; }
    }
}
