using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SniperLog.Config;
using SniperLog.Pages;
using SniperLog.Pages.Other;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Pages.ShootingRanges.Subranges;
using SniperLog.Services.ConnectionToServer;
using SniperLog.Services.Serialization;
using SniperLog.ViewModels;
using SniperLog.ViewModels.Other;
using SniperLog.ViewModels.SRanges;
using SniperLog.ViewModels.SRanges.Subranges;
using System.Net;

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


            var cacherTypes = typeof(IDataAccessObject).Assembly.GetTypes().Where(n => !n.IsAbstract && n.GetInterface("IDataAccessObject") != null);

            foreach (Type type in cacherTypes)
            {
                Type dataCacherServiceTypeGenerics = typeof(DataCacherService<>).MakeGenericType(type);
                var cacher = Activator.CreateInstance(dataCacherServiceTypeGenerics);
                builder.Services.AddSingleton(dataCacherServiceTypeGenerics, cacher);
            }


            #endregion

            #region View Models

            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<InitialSetupPopupPageViewModel>();

            builder.Services.AddSingleton<SRangesPageViewModel>();
            builder.Services.AddSingleton<SRangesAddOrEditPageViewModel>();
            builder.Services.AddSingleton<SRangeDetailsPageViewModel>();

            builder.Services.AddSingleton<SubRangesPageViewModel>();
            builder.Services.AddSingleton<SubRangeAddOrEditPageViewModel>();

            #endregion

            #region Pages

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<InitialSetupPopupPage>();

            builder.Services.AddSingleton<SRangesPage>();
            builder.Services.AddSingleton<SRangesAddOrEditPage>();
            builder.Services.AddSingleton<SRangeDetailsPage>();

            builder.Services.AddSingleton<SubRangesPage>();
            builder.Services.AddSingleton<SubRangeAddOrEditPage>();

            #endregion

            #region Processors

            builder.Services.AddSingleton<CsvProcessor>();

            #endregion

            #region Other

            builder.Services.AddSingleton<ValidatorService>();

            AppConfig config = ApplicationConfigService.GetConfig<AppConfig>();
            ConnectionToDataServer connectionToDataServer = new ConnectionToDataServer()
            {
                //HostName = config.ServerHostname,
                IpAddress = IPAddress.Parse("10.0.2.2"),
                Port = config.ServerPort,
            };

            builder.Services.AddSingleton<ConnectionToDataServer>(connectionToDataServer);

            #endregion

            return builder;
        }
    }
}
