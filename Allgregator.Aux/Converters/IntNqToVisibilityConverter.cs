using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public class IntNqToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value == (int)parameter ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
