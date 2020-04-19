using System;
using System.Globalization;
using System.Windows.Data;

namespace Allgregator.Converters {
    class BooleanToContentConverter : IValueConverter {
        public object TrueContent { get; set; }
        public object FalseContent { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return TrueContent;
            }

            return FalseContent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
