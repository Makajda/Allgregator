using Allgregator.Aux.Models;
using System;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Allgregator.Sts.Models {
    public class PaletteColor : IName {
        private readonly Lazy<Brush> brush;
        public PaletteColor() {
            brush = new Lazy<Brush>(() => new SolidColorBrush(Color.FromRgb(R, G, B)));
        }

        public string Name { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        [JsonIgnore]
        public Brush Brush { get => brush.Value; }
    }
}
