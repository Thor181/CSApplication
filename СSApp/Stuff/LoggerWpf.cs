using CSLibrary.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СSApp.Stuff
{
    public class LoggerWpf : Logger
    {
        public delegate void MessageEventHandler(string message, LogLevel logLevel, Exception e = null);
        public event MessageEventHandler MessageReceived;

        public override string Log(string message, LogLevel level, Exception e = null)
        {
            MessageReceived?.Invoke(message, level, e);

            return base.Log(message, level, e);
        }
    }
}
