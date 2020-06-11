using Allgregator.Aux.Models;
using System.Windows.Media;

namespace Allgregator.Sts.Model {
    public class PaletteColor : IName {
        public string Name { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public Brush Brush { get => new SolidColorBrush(Color.FromRgb(R, G, B)); }//todo
    }
}
