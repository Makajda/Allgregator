using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Allgregator.Aux.Converters {
    public sealed class RgbToColorConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length > 2) {
                if (values[0] is byte r) {
                    if (values[1] is byte g) {
                        if (values[2] is byte b) {
                            return Color.FromRgb(r, g, b);
                        }
                    }
                }
            }

            return default(Color);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
