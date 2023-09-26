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
        public Queue<Action> Actions = new Queue<Action>();

        public override string Log(string message, LogLevel level, Exception e = null)
        {
            var formattedMessage = base.Log(message, level, e);

            Render.WriteLine(formattedMessage, level.GetConsoleColor());

            return formattedMessage;
        }
    }
}
