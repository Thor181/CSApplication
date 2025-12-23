
using CSApp.V2w.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSApp.V2w.Utils
{
    public static class Extensions
    {
        public static IConfigurationBuilder AddCustomJsonFile(this IConfigurationBuilder builder)
        {
            const string appSettingsPath = "_appSettings.json";
            if (File.Exists(appSettingsPath))
            {
                var json = File.ReadAllText(appSettingsPath);
                var path = JsonSerializer.Deserialize<CustomAppSettingsPath>(json)?.Path;

                if (!string.IsNullOrEmpty(path))
                {
                    if (!File.Exists(path))
                        throw new FileNotFoundException($"The custom app settings file was not found at the specified path: {path}");

                    builder.AddJsonFile(path);
                }
                else
                {
                    builder.AddJsonFile(appSettingsPath);
                }
            }

            return builder;
        }
    }
}
