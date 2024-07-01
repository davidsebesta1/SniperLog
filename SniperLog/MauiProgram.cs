using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SniperLog.Config;
using SniperLog.Pages;
using SniperLog.Services.ConnectionToServer;
using SniperLog.Services.Serialization;
using SniperLog.ViewModels;

namespace SniperLog
{
    public static class MauiProgram
    {
        public static MauiApp ApplicationInstance { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries.Init();
            ApplicationConfigService.Init();

            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                 .UseMauiApp<App>()
                 .UseMauiCommunityToolkit()
                 .SetupServices()
                 .ConfigureMopups()
                 .ConfigureFonts(fonts =>
                 {
                     fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                 });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            ApplicationInstance = builder.Build();

            ServicesHelper.Init(ApplicationInstance.Services);

            SqliteDatabaseUpdatePatcher.CheckForUpdates();
            SqLiteDatabaseConnection.Init();

            return ApplicationInstance;
        }

        public static MauiAppBuilder SetupServices(this MauiAppBuilder builder)
        {
            #region Data Cacher Services



            #endregion

            #region View Models

            builder.Services.AddSingleton<MainPageViewModel>();

            #endregion

            #region Pages

            builder.Services.AddSingleton<MainPage>();


            #endregion

            #region Processors

            builder.Services.AddSingleton<CsvProcessor>();

            #endregion

            #region Other

            builder.Services.AddTransient<ValidatorService>();

            ConnectionToDataServer connectionToDataServer = new ConnectionToDataServer();
            AppConfig config = ApplicationConfigService.GetConfig<AppConfig>();
            connectionToDataServer.HostName = config.ServerHostname;
            connectionToDataServer.Port = config.ServerPort;

            builder.Services.AddSingleton<ConnectionToDataServer>(connectionToDataServer);

            #endregion

            return builder;
        }
    }
}
