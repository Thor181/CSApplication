using Avalonia;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CSApp.V2a
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                try
                {
                    var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "CSApp";
                    var reportsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName, "crash_reports");
                    Directory.CreateDirectory(reportsDir);

                    var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.txt";
                    var filePath = Path.Combine(reportsDir, fileName);

                    var sb = new StringBuilder();
                    sb.AppendLine($"Timestamp (UTC): {DateTime.UtcNow:O}");
                    sb.AppendLine($"Application: {appName}");
                    sb.AppendLine($"Version: {Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0)}");
                    sb.AppendLine($"Args: {string.Join(" ", args ?? Array.Empty<string>())}");
                    sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
                    sb.AppendLine($"Runtime: {RuntimeInformation.FrameworkDescription}");
                    sb.AppendLine();
                    sb.AppendLine("Exception:");
                    sb.AppendLine(ex.ToString());

                    File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
                catch
                {
                    // Если логирование краша не удалось — подавляем ошибки, чтобы не скрыть оригинальную причину.
                }

                throw;
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
