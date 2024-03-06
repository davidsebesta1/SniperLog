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

            builder.Services.AddSingleton<DataService<ShootingRange>>();

            builder.Services.AddSingleton<ShootingRangeViewModel>();
            builder.Services.AddSingleton<ShootingRangeDetailsViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ShootingRangesPage>();
            builder.Services.AddTransient<ShootingRangeDetailsPage>();
            builder.Services.AddTransient<ShootingRangeSubRangesPage>();

            ApplicationInstance = builder.Build();

            SqliteDatabaseUpdatePatcher.CheckForUpdates();
            return ApplicationInstance;
        }
    }
}