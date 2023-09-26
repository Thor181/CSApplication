using CSLibrary.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApplication
{
    public static class Extensions
    {
        public static ConsoleColor GetConsoleColor(this LogLevel logLevel)
        {

            return logLevel switch
            {
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Fatal => ConsoleColor.DarkRed,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warn => ConsoleColor.Yellow,
                LogLevel.Success => ConsoleColor.Green,
                _ => ConsoleColor.White,
            };
        }
    }
}
