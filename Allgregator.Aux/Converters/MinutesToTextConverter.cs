using System;
using System.Globalization;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class MinutesToTextConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is double minutes) {
                return $"{(int)minutes / 60}:{minutes % 60}";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
