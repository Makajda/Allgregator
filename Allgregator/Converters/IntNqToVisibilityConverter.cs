using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Allgregator.Converters {
    class IntNqToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is int v) {
                if (parameter is int p) {
                    return v == p ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
