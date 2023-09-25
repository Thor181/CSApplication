using CSLibrary.Results;
using System.Text.Json;

namespace CSLibrary
{
    public class AppConfig
    {
        #region Instance

        private static AppConfig _instance = null!;
        public static AppConfig Instance => _instance ??= new AppConfig();

        #endregion

        #region Ports names

        public string PortInputName { get; set; } = "COM1";
        public string PortOutputName { get; set; } = "COM2";
        public string PortQR1Name { get; set; } = "COM3";
        public string PortQR2Name { get; set; } = "COM4";

        #endregion

        public string PointIdentifier { get; set; } = "PLACE1";

        public List<string> FNNumbers { get; set; } = new List<string>();

        public DbConnectionString DbConnectionString { get; set; } = new DbConnectionString();

        private string _configPath = "Config\\Config.json";

        public void Initialize()
        {
            CheckDirectoryExist();
            CheckFileExist();

            Load();
        }

        private void CheckDirectoryExist()
        {
            var fileName = _configPath;
            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);
        }

        private void CheckFileExist()
        {
            var isExists = File.Exists(_configPath);
            if (!isExists)
                Save();
        }

        public void Save()
        {
            var serialized = JsonSerializer.Serialize(Instance);
            File.WriteAllText(_configPath, serialized);
        }

        public void Load()
        {
            var json = File.ReadAllText(_configPath);
            var deserialized = JsonSerializer.Deserialize<AppConfig>(json);
            _instance = deserialized;
        }

        public BaseResult ValidateConfig()
        {
            var validationResult = new BaseResult();

            if (string.IsNullOrEmpty(PortInputName))
                validationResult.MessageBuilder.AppendLine($"Поле {nameof(PortInputName)} не может быть пустым");

            if (string.IsNullOrEmpty(PortOutputName))
                validationResult.MessageBuilder.AppendLine($"Поле {nameof(PortOutputName)} не может быть пустым");

            if (string.IsNullOrEmpty(PortQR1Name))
                validationResult.MessageBuilder.AppendLine($"Поле {nameof(PortQR1Name)} не может быть пустым");

            if (string.IsNullOrEmpty(PortQR2Name))
                validationResult.MessageBuilder.AppendLine($"Поле {nameof(PortQR2Name)} не может быть пустым");



            return validationResult;
        }
    }
}
