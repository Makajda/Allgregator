using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Allgregator.Aux.Converters {
    public sealed class BooleanToBrushConverter : IValueConverter {
        public Brush TrueBrush { get; set; }
        public Brush FalseBrush { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return TrueBrush;
            }

            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
