using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Allgregator.Converters {
    public class NullToBrushConverter : IValueConverter {
        public Brush BrushNull { get; set; }
        public Brush BrushNotNull { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? BrushNull : BrushNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
