using CSLibrary.Log;
using System;
using System.Windows.Media;

namespace СSApp
{
    public static class Extensions
    {
        public static Brush GetColor(this LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Error => new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0)),
                LogLevel.Fatal => new SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 7, 0)),
                LogLevel.Info => new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)),
                LogLevel.Warn => new SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 164, 5)),
                LogLevel.Success => new SolidColorBrush(System.Windows.Media.Color.FromRgb(27, 140, 53)),
                _ => new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)),
            };
        }
    }
}
