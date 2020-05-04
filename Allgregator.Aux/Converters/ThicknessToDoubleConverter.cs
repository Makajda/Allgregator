using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class ThicknessToDoubleConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Thickness t) {
                return t.Left;
            }

            return default(double);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is double d) {
                return new Thickness(d);
            }

            return default(Thickness);
        }
    }
}
