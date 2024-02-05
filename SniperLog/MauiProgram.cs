using Microsoft.Extensions.Logging;
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
            var builder = MauiApp.CreateBuilder();
            builder

                .UseMauiApp<App>()
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

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ShootingRangesPage>();

            App = builder.Build();
            return App;
        }
    }
}
