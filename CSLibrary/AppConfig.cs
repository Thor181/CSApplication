﻿using CSLibrary.Log;
using CSLibrary.Results;
using CSLibrary.Stuff;
using System.Text.Json;

namespace CSLibrary
{
    public class AppConfig
    {
        #region Instance

        private static AppConfig _instance = new AppConfig();
        public static AppConfig Instance => _instance;

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

            if (deserialized == null)
            {
                Logger.Instance.Log("Конфиг не распознан", LogLevel.Fatal);
                return;
            }

            ValidateConfig(deserialized);

            _instance = deserialized;
        }

        public void ValidateConfig(AppConfig instance)
        {
            var validationEmptyStrings = Validation.StringsIsNullOrEmpty<AppConfig>(instance, x => x.PortInputName,
                                                                                        x => x.PortOutputName,
                                                                                        x => x.PortQR1Name,
                                                                                        x => x.PortQR2Name,
                                                                                        x => x.PointIdentifier);

            if (!validationEmptyStrings.IsSuccses)
                Logger.Instance.Log(validationEmptyStrings.MessageBuilder.ToString(), LogLevel.Error);

            var stringsLengthValidation = Validation.AllStringsEquals(16, instance.FNNumbers);

            if (!stringsLengthValidation.IsSuccses)
                Logger.Instance.Log(stringsLengthValidation.MessageBuilder.ToString(), LogLevel.Error);

            var moreThanOneSamePortNames = Validation.StringsCountMoreThanOne(instance.PortInputName,
                                                                              instance.PortOutputName,
                                                                              instance.PortQR1Name,
                                                                              instance.PortQR2Name);

            if (moreThanOneSamePortNames)
                Logger.Instance.Log("Обнаружены одинаковые имена портов", LogLevel.Error);

            var dbConnectionStringNotValid = instance.DbConnectionString.TrustedConnection == true
                                            && !string.IsNullOrEmpty(instance.DbConnectionString.User);

            if (!validationEmptyStrings.IsSuccses || !stringsLengthValidation.IsSuccses || moreThanOneSamePortNames)
                Environment.Exit(1);

            if (instance.FNNumbers.Count == 0)
                Logger.Instance.Log($"Не обнаружены номера FN ({nameof(instance.FNNumbers)})", LogLevel.Warn);

            if (dbConnectionStringNotValid)
                Logger.Instance.Log($"Поле {nameof(DbConnectionString.User)} и {nameof(DbConnectionString.Password)} не имеют значения, " +
                    $"если {nameof(instance.DbConnectionString.TrustedConnection)} равно \"true\"", LogLevel.Warn);
        }
    }
}
