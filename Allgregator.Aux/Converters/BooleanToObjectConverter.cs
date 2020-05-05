using System;
using System.Globalization;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class BooleanToObjectConverter : IValueConverter {
        public object True { get; set; }
        public object False { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return True;
            }

            return False;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
