using System;
using System.Diagnostics.CodeAnalysis;

namespace Allgregator.Sts.Model {
    public class Symbol : IEquatable<Symbol> {
        public string Name { get; set; }
        public char Char { get; set; }

        public bool Equals([AllowNull] Symbol other) =>
            Char == other.Char;

        public override bool Equals(object obj) =>
            Equals((Symbol)obj);

        public override int GetHashCode() =>
            $"{Name}{Char}".GetHashCode();

        public static bool operator ==(Symbol symbol1, Symbol symbol2) {
            if (((object)symbol1) == null || ((object)symbol2) == null)
                return Object.Equals(symbol1, symbol2);

            return symbol1.Equals(symbol2);
        }

        public static bool operator !=(Symbol symbol1, Symbol symbol2) {
            if (((object)symbol1) == null || ((object)symbol2) == null)
                return !Object.Equals(symbol1, symbol2);

            return !(symbol1.Equals(symbol2));
        }
    }
}
