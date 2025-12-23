using CSApp.V2w.Services;
using CSLibrary.V2;
using CSLibrary.V2.Data.Logic;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Navigation;
using CSApp.V2w.Services.Options;
using CSApp.V2w.ViewModels;

namespace CSApp.V2w
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("C:\\settings\\_appSettings.json").Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.Section));
            services.Configure<UiOptions>(configuration.GetSection(UiOptions.Section));
            services.Configure<DbConnectionOptions>(configuration.GetSection(DbConnectionOptions.Section));
            services.Configure<PortWorkerOptions>(configuration.GetSection(PortWorkerOptions.Section));
            //Регистрируем IPortWorkerOptions для внедрения зависимостей, т.к. используется в CSLibrary.V2 (который не имеет доступа до PortWorkerOptions)
            services.AddScoped<IPortWorkerOptions>((serviceProvider) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<PortWorkerOptions>>();
                return options.Value;
            });

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
                }).CreateLogger(Utils.Constants.AppName);

                return logger;
            });

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<MainScreenService>();
            services.AddScoped<AudioPlayer>();
            services.AddScoped<PortWorker>();
            services.AddScoped<PersistentValues>();
            services.AddTransient<CardEventLogic>();
            services.AddTransient<HelperEntityLogic<Place>>();
            services.AddTransient<HelperEntityLogic<EventsType>>();
            services.AddTransient<HelperEntityLogic<CSLibrary.V2.Data.Models.Point>>();
            services.AddTransient<HelperEntityLogic<PayType>>();
            services.AddTransient<InitializationLogic>();
            services.AddTransient<QREventLogic>();
            services.AddTransient<UserLogic>();
            services.AddTransient<OperatorEventLogic>();

            return services;
        }

    }

}
