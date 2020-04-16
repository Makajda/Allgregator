using System;
using System.Globalization;
using System.Windows.Data;

namespace Allgregator.Converters {
    public class DateTimeOffsetToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is DateTimeOffset dt) {
                return dt.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string s && s != null) {
                if (DateTimeOffset.TryParse(s, out DateTimeOffset dt)) {
                    return dt;
                }
            }

            return value;
        }
    }
}
