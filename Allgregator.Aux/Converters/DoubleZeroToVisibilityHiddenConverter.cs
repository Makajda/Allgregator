using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class DoubleZeroToVisibilityHiddenConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Math.Abs((double)value) < double.Epsilon ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
