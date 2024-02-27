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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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

            ApplicationInstance = builder.Build();

            SqliteDatabaseUpdatePatcher.CheckForUpdates();
            return ApplicationInstance;
        }
    }
}
