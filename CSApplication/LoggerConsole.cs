using CSLibrary.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApplication
{
    public class LoggerConsole : Logger
    {
        public override string Log(string message, LogLevel level, Exception e = null)
        {
            var formattedMessage = base.Log(message, level, e);

            Render.WriteLine(formattedMessage, ConsoleColor.Red);

            return formattedMessage;
        }
    }
}
