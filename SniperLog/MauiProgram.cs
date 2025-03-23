using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SniperLog.Config;
using SniperLog.Extensions;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using SniperLog.Pages;
using SniperLog.Pages.Firearms;
using SniperLog.Pages.Firearms.Bullets;
using SniperLog.Pages.Manufacturers;
using SniperLog.Pages.Manufacturers.FirearmManufacturers;
using SniperLog.Pages.Other;
using SniperLog.Pages.Records;
using SniperLog.Pages.Records.Popups;
using SniperLog.Pages.Reticles;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Pages.ShootingRanges.Subranges;
using SniperLog.Pages.Sights;
using SniperLog.Pages.Sights.ClickSettings;
using SniperLog.Services.ConnectionToServer;
using SniperLog.Services.Serialization;
using SniperLog.ViewModels;
using SniperLog.ViewModels.Firearms;
using SniperLog.ViewModels.Manufacturers;
using SniperLog.ViewModels.Manufacturers.FirearmManufacturers;
using SniperLog.ViewModels.Manufacturers.SightManufacturers;
using SniperLog.ViewModels.Other;
using SniperLog.ViewModels.Records;
using SniperLog.ViewModels.Reticles;
using SniperLog.ViewModels.Sights;
using SniperLog.ViewModels.Sights.ClickSettings;
using SniperLog.ViewModels.SRanges;
using SniperLog.ViewModels.SRanges.Subranges;
using SniperLog.ViewModels.Firearms.Bullets;
using System.Net;
using SniperLog.ViewModels.Manufacturers.BulletManufacturers;
using SniperLog.ViewModels.Manufacturers.AmmunitionManufacturers;
using SniperLog.ViewModels.Firearms.MuzzleVelocities;
using SniperLog.Pages.Firearms.MuzzleVelocities;
using SniperLog.Services.AI;

namespace SniperLog
{
    /// <summary>
    /// Main class of the program aswell as entry point.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Singleton instance of the <see cref="MauiApp"/>
        /// </summary>
        public static MauiApp ApplicationInstance { get; private set; }

        /// <summary>
        /// Inicializes the builder and returns app singleton reference.
        /// </summary>
        /// <returns>App singleton reference.</returns>
        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries.Init();
            ApplicationConfigService.Init();
            AppContext.SetSwitch("System.Reflection.NullabilityInfoContext.IsSupported", true);

            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                 .UseMauiApp<App>()
                 .UseMauiCommunityToolkit()
                 .SetupServices()
                 .UseSkiaSharp()
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

        /// <summary>
        /// Extension method to setup services for this application.
        /// </summary>
        /// <param name="builder">This app builder.</param>
        /// <returns>Builder with setup services.</returns>
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

            builder.Services.AddSingleton<ManufacturersPageViewModel>();

            builder.Services.AddSingleton<FirearmManuPageViewModel>();
            builder.Services.AddSingleton<SightManuPageViewModel>();
            builder.Services.AddSingleton<ManufacturerAddOrEditPageViewModel>();
            builder.Services.AddSingleton<BulletManuPageViewModel>();
            builder.Services.AddSingleton<AmmoManuPageViewModel>();

            builder.Services.AddTransient<CustomPickerPopupViewModel>();

            builder.Services.AddSingleton<SightsPageViewModel>();
            builder.Services.AddSingleton<SightAddOrEditPageViewModel>();

            builder.Services.AddSingleton<ReticlesPageViewModel>();
            builder.Services.AddSingleton<ReticleAddOrEditPageViewModel>();

            builder.Services.AddSingleton<SightClickSettingsPageViewModel>();
            builder.Services.AddSingleton<SightClickSettingAddOrEditPageViewModel>();

            builder.Services.AddSingleton<FirearmsPageViewModel>();
            builder.Services.AddSingleton<FirearmAddOrEditPageViewModel>();

            builder.Services.AddSingleton<RecordsPageViewModel>();
            builder.Services.AddSingleton<RecordDetailsPageViewModel>();
            builder.Services.AddSingleton<WeatherEditPopupPageViewModel>();

            builder.Services.AddSingleton<CustomImageEditorPopupViewModel>();

            builder.Services.AddSingleton<CustomDatetimePickerPopupViewModel>();

            builder.Services.AddSingleton<AmmunitionsPageViewModel>();
            builder.Services.AddSingleton<AmmunitionAddOrEditPageViewModel>();

            builder.Services.AddSingleton<BulletsPageViewModel>();
            builder.Services.AddSingleton<BulletAddOrEditPageViewModel>();

            builder.Services.AddSingleton<MuzzleVelocitiesPageViewModel>();
            builder.Services.AddSingleton<MuzzleVelocityAddOrEditPageViewModel>();

            builder.Services.AddSingleton<ImportExportPageViewModel>();

            #endregion

            #region Pages

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<InitialSetupPopupPage>();

            builder.Services.AddSingleton<SRangesPage>();
            builder.Services.AddSingleton<SRangesAddOrEditPage>();
            builder.Services.AddSingleton<SRangeDetailsPage>();

            builder.Services.AddSingleton<SubRangesPage>();
            builder.Services.AddSingleton<SubRangeAddOrEditPage>();

            builder.Services.AddSingleton<ManufacturersPage>();

            builder.Services.AddSingleton<BulletManuPage>();
            builder.Services.AddSingleton<SightManuPage>();
            builder.Services.AddSingleton<AmmoManuPage>();
            builder.Services.AddSingleton<FirearmManuPage>();
            builder.Services.AddSingleton<ManufacturerAddOrEditPage>();

            builder.Services.AddSingleton<SightsPage>();
            builder.Services.AddSingleton<SightAddOrEditPage>();

            builder.Services.AddSingleton<ReticlesPage>();
            builder.Services.AddSingleton<ReticleAddOrEditPage>();

            builder.Services.AddSingleton<SightClickSettingsPage>();
            builder.Services.AddSingleton<SightClickSettingAddOrEditPage>();

            builder.Services.AddSingleton<FirearmsPage>();
            builder.Services.AddSingleton<FirearmAddOrEditPage>();

            builder.Services.AddSingleton<RecordsPage>();
            builder.Services.AddSingleton<RecordDetailsPage>();
            builder.Services.AddSingleton<WeatherEditPopupPage>();

            builder.Services.AddSingleton<CustomDatetimePickerPopup>();

            builder.Services.AddSingleton<AmmunitionsPage>();
            builder.Services.AddSingleton<AmmunitionAddOrEditPage>();

            builder.Services.AddSingleton<BulletsPage>();
            builder.Services.AddSingleton<BulletAddOrEditPage>();

            builder.Services.AddSingleton<MuzzleVelocitiesPage>();
            builder.Services.AddSingleton<MuzzleVelocityAddOrEditPage>();

            builder.Services.AddSingleton<ImportExportPage>();

            #endregion

            #region Processors

            builder.Services.AddSingleton<CsvProcessor>();

            #endregion

            #region Other

            builder.Services.AddSingleton<ValidatorService>();
            builder.Services.AddSingleton<BulletHoleDetectionService>();

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
