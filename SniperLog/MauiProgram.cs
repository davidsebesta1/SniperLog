using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SniperLog.Config;
using SniperLog.Pages;
using SniperLog.Pages.Other;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Services.ConnectionToServer;
using SniperLog.Services.Serialization;
using SniperLog.ViewModels;
using SniperLog.ViewModels.Other;
using SniperLog.ViewModels.SRanges;

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

            
            var types = typeof(IDataAccessObject).Assembly.GetTypes().Where(n => !n.IsAbstract && n.GetInterface("IDataAccessObject") != null);

            foreach (Type type in types)
            {
                Type dataCacherServiceTypeGenerics = typeof(DataCacherService<>).MakeGenericType(type);
                var cacher = Activator.CreateInstance(dataCacherServiceTypeGenerics);
                builder.Services.AddSingleton(dataCacherServiceTypeGenerics, cacher);
            }
            

            #endregion

            #region View Models

            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<SRangesPageViewModel>();
            builder.Services.AddSingleton<InitialSetupPopupPageViewModel>();
            builder.Services.AddSingleton<SRangesAddOrEditPageViewModel>();

            #endregion

            #region Pages

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<SRangesPage>();
            builder.Services.AddSingleton<InitialSetupPopupPage>();
            builder.Services.AddSingleton<SRangesAddOrEditPage>();

            #endregion

            #region Processors

            builder.Services.AddSingleton<CsvProcessor>();

            #endregion

            #region Other

            builder.Services.AddTransient<ValidatorService>();

            AppConfig config = ApplicationConfigService.GetConfig<AppConfig>();
            ConnectionToDataServer connectionToDataServer = new ConnectionToDataServer()
            {
                HostName = config.ServerHostname,
                Port = config.ServerPort,
            };

            builder.Services.AddSingleton<ConnectionToDataServer>(connectionToDataServer);

            #endregion

            return builder;
        }
    }
}
