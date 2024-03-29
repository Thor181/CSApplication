﻿using CSLibrary.Log;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace СSApp.Converters
{
    [ValueConversion(typeof(LogLevel), typeof(Brush))]
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((LogLevel)value).GetColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
