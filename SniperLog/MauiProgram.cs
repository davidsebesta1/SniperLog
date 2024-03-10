using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SniperLog.Models;
using SniperLog.Pages;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Services;
using SniperLog.Services.Database;
using SniperLog.ViewModels;

namespace SniperLog
{
    public static class MauiProgram
    {
        public static MauiApp ApplicationInstance;

        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries.Init();
            ApplicationConfigService.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMopups()
                .UseMauiMaps()
                .ConfigureEssentials(essentials =>
                {
                    essentials.UseVersionTracking();
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.SetupServices();

            ApplicationInstance = builder.Build();

            ServicesHelper.Init(ApplicationInstance.Services);

            SqliteDatabaseUpdatePatcher.CheckForUpdates();
            return ApplicationInstance;
        }

        public static void SetupServices(this MauiAppBuilder builder)
        {
            #region Data Cacher Services
            builder.Services.AddSingleton<DataCacherService<ShootingRange>>();
            builder.Services.AddSingleton<DataCacherService<SubRange>>();
            #endregion

            #region View Models
            builder.Services.AddSingleton<ShootingRangeViewModel>();
            builder.Services.AddSingleton<ShootingRangeDetailsViewModel>();
            builder.Services.AddSingleton<SubRangesSettingsViewModel>();
            #endregion

            #region Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ShootingRangesPage>();
            builder.Services.AddTransient<ShootingRangeDetailsPage>();
            builder.Services.AddTransient<ShootingRangeSubRangesPage>();
            #endregion
        }
    }
}