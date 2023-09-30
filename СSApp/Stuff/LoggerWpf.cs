using CSLibrary.Log;

namespace СSApp.Stuff
{
    public class LoggerWpf : Logger
    {
        public delegate void MessageEventHandler(string message, LogLevel logLevel, Exception e = null);
        public event MessageEventHandler MessageReceived;

        public override string Log(string message, LogLevel level, Exception e = null)
        {
            var formattedMessage = base.Log(message, level, e);
            MessageReceived?.Invoke(formattedMessage, level, e);

            return formattedMessage;
        }
    }
}
