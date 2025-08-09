using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CSApp.V2a.Services;
using CSApp.V2a.Services.Options;
using CSApp.V2a.Utils;
using CSApp.V2a.ViewModels;
using CSApp.V2a.Views;
using CSLibrary.V2;
using CSLibrary.V2.Data.Models;
using Karambolo.Extensions.Logging.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Linq;

namespace CSApp.V2a
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
                };

            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("_appSettings.json").Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.Section));
            services.Configure<UiOptions>(configuration.GetSection(UiOptions.Section));
            services.Configure<DbConnectionOptions>(configuration.GetSection(DbConnectionOptions.Section));
            services.Configure<PortWorkerOptions>(configuration.GetSection(PortWorkerOptions.Section));

            services.AddDbContext<MfraDbContext>((serviceProvider, builder) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<DbConnectionOptions>>().Value;
                var dbConnectionStringBuilder = new DbConnectionStringBuilder();

                dbConnectionStringBuilder.Add("Server", options.Server);
                dbConnectionStringBuilder.Add("Database", options.Database);

                if (!options.TrustedConnection)
                {
                    dbConnectionStringBuilder.Add("User Id", options.User);
                    dbConnectionStringBuilder.Add("Password", options.Password);
                }
                else
                {
                    dbConnectionStringBuilder.Add("Trusted_Connection", options.TrustedConnection);
                }

                dbConnectionStringBuilder.Add("Encrypt", options.Encrypt);
                dbConnectionStringBuilder.Add("TrustServerCertificate", true);

                var connectionString = dbConnectionStringBuilder.ToString();

                builder.UseLazyLoadingProxies().UseSqlServer(connectionString);
            });

            services.AddSingleton<ILogger>(provider =>
            {
                var loggingOptions = provider.GetRequiredService<IOptions<LoggingOptions>>().Value;

                var logger = LoggerFactory.Create(x =>
                {
                    x.AddFile(y =>
                    {
                        y.BasePath = loggingOptions.Folder;
                        y.RootPath = loggingOptions.RootPath == "./" ? AppContext.BaseDirectory : loggingOptions.RootPath;
                        y.Files = [new LogFileOptions() {
                            Path = loggingOptions.FileFormat,
                            MaxFileSize = loggingOptions.MaxFileSize
                        }];
                    });
                }).CreateLogger(Constants.AppName);

                return logger;
            });

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<MainScreenService>();
            services.AddScoped<AudioPlayer>();
            services.AddScoped<PortWorker>();


            return services;
        }
    }
}