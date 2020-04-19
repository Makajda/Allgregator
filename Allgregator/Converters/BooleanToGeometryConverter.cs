using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Allgregator.Converters {
    public class BooleanToGeometryConverter : IValueConverter {
        public Geometry TrueGeometry { get; set; }
        public Geometry FalseGeometry { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return TrueGeometry;
            }

            return FalseGeometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
