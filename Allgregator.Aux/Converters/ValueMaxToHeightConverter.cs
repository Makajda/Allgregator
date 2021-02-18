using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Allgregator.Aux.Converters {
    public sealed class ValueMaxToHeightConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length > 2) {
                if (values[0] is int max && max != 0) {
                    if (values[1] is int value) {
                        if (values[2] is Grid grid && grid.RowDefinitions.Count > 0) {
                            return grid.RowDefinitions[0].ActualHeight * value / max;
                        }
                    }
                }
            }

            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
