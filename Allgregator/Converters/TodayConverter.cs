﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Allgregator.Converters {
    public class TodayConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is DateTimeOffset d) {
                if (d != DateTimeOffset.MinValue) {
                    var now = DateTimeOffset.Now;
                    var date = now.Date == d.Date ? null : $"{d.Day:D2}.{d.Month:D2} ";
                    var time = d.TimeOfDay;
                    var valret = $"{date}{time.Hours:D2}:{time.Minutes:D2}";
                    return valret;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
