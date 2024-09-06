using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SniperLog.Config;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using SniperLog.Pages;
using SniperLog.Pages.Firearms;
using SniperLog.Pages.Manufacturers;
using SniperLog.Pages.Manufacturers.FirearmManufacturers;
using SniperLog.Pages.Other;
using SniperLog.Pages.Records;
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

            builder.Services.AddSingleton<ManufacturersPageViewModel>();
            builder.Services.AddSingleton<FirearmManuPageViewModel>();
            builder.Services.AddSingleton<SightManuPageViewModel>();
            builder.Services.AddSingleton<ManufacturerAddOrEditPageViewModel>();

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

            builder.Services.AddSingleton<CustomImageEditorPopupViewModel>();

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
            builder.Services.AddSingleton<FirearmManuPage>();
            builder.Services.AddSingleton<SightManuPage>();
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
