using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace СSApp
{
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var appName = (Assembly.GetEntryAssembly()?.GetName().Name ?? "CSApp") +  "_old";
            var reportsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName, "crash_reports");
            Directory.CreateDirectory(reportsDir);

            var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.txt";
            var filePath = Path.Combine(reportsDir, fileName);

            var sb = new StringBuilder();
            sb.AppendLine($"Timestamp (UTC): {DateTime.UtcNow:O}");
            sb.AppendLine($"Application: {appName}");
            sb.AppendLine($"Version: {Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0)}");
            sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"Runtime: {RuntimeInformation.FrameworkDescription}");
            sb.AppendLine();
            sb.AppendLine("Exception:");
            sb.AppendLine(ex.ToString());

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }

}
