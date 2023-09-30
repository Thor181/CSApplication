using CSLibrary.Stuff;

namespace CSLibrary.Log
{
    public class Logger
    {
        public static Logger Instance { get; set; }

        private string _logsPath = $"{Constants.MainFolderPath}\\Logs\\";
        private string _extension = ".log";

        public Logger()
        {
            CheckDirectoryExist();
            Instance = this;
        }

        public virtual string Log(string message, LogLevel level, Exception e = null)
        {
            var formattedMessage = $"[{DateTime.Now}] ({level}) | {message}";

            if (e != null)
                formattedMessage += $" | {e.Message}";

            AppendLines(formattedMessage);

            return formattedMessage;
        }

        private void AppendLines(params string[] formattedMessage)
        {
            var fileName = GetFileName();
            File.AppendAllLines(fileName, formattedMessage);
        }

        private void CheckDirectoryExist()
        {
            var fileName = GetFileName();
            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);
        }

        private string GetFileName()
        {
            var path = Path.Combine(_logsPath, DateTime.Today.ToShortDateString().ToString());
            var fileName = $"{path}{_extension}";

            return fileName;
        }
    }
}
