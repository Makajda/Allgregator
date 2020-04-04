using Allgregator.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Allgregator.Converters {
    public class UriToImageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return CacheImageHelper.Get(value as Uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
