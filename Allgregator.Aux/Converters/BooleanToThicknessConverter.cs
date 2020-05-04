using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class BooleanToThicknessConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                if (parameter != null && double.TryParse(parameter.ToString(), out double t)) {
                    return new Thickness(t);
                }

                return new Thickness(1);
            }

            return default(Thickness);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
