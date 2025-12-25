using CSLibrary.Log;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace СSApp.Converters
{
    [ValueConversion(typeof(LogLevel), typeof(Brush))]
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is not LogLevel logLevel)
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            return logLevel.GetColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
