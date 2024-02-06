using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SniperLog.Models;
using SniperLog.Pages;
using SniperLog.Services;
using SniperLog.ViewModels;

namespace SniperLog
{
    public static class MauiProgram
    {
        public static MauiApp App;

        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries.Init();

            var builder = MauiApp.CreateBuilder();
            builder

                .UseMauiApp<App>()
                .ConfigureMopups()
                .UseMauiMaps()
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

            App = builder.Build();
            return App;
        }
    }
}
