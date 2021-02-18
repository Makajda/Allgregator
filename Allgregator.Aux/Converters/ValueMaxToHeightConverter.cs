using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class ValueMaxToHeightConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var defaultMin = 60d;

            if (values.Length > 2) {
                if (values[0] is int max && max != 0) {
                    if (values[1] is int value) {
                        if (values[2] is double height) {
                            var result = height * value / max;
                            if (parameter != null && double.TryParse(parameter.ToString(), out double min))
                                defaultMin = min;

                            return Math.Max(defaultMin, result);
                        }
                    }
                }
            }

            return defaultMin;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
