using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Mopups.Hosting;
using SniperLog.Models;
using SniperLog.Pages;
using SniperLog.Services;
using SniperLog.ViewModels;
using System.Resources;

namespace SniperLog
{
    public static class MauiProgram
    {
        public static MauiApp ApplicationInstance;

        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries.Init();
            ApplicationConfigService.Init();

            SqliteDatabaseUpdatePatcher.CheckForUpdates();

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

            builder.Services.AddSingleton<DataFetcherService<ShootingRange>>();

            builder.Services.AddSingleton<ShootingRangeViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ShootingRangesPage>();

            ApplicationInstance = builder.Build();

            return ApplicationInstance;
        }
    }
}
