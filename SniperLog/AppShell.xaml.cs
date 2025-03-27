using SniperLog.Pages.Firearms;
using SniperLog.Pages.Firearms.Bullets;
using SniperLog.Pages.Firearms.MuzzleVelocities;
using SniperLog.Pages.Manufacturers;
using SniperLog.Pages.Manufacturers.FirearmManufacturers;
using SniperLog.Pages.Other;
using SniperLog.Pages.Records;
using SniperLog.Pages.Reticles;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Pages.ShootingRanges.Subranges;
using SniperLog.Pages.Sights;
using SniperLog.Pages.Sights.ClickSettings;

namespace SniperLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SRangesAddOrEditPage), typeof(SRangesAddOrEditPage));
            Routing.RegisterRoute(nameof(SRangeDetailsPage), typeof(SRangeDetailsPage));
            Routing.RegisterRoute(nameof(SubRangesPage), typeof(SubRangesPage));
            Routing.RegisterRoute(nameof(SubRangeAddOrEditPage), typeof(SubRangeAddOrEditPage));

            Routing.RegisterRoute(nameof(ManufacturersPage), typeof(ManufacturersPage));
            Routing.RegisterRoute(nameof(BulletManuPage), typeof(BulletManuPage));
            Routing.RegisterRoute(nameof(SightManuPage), typeof(SightManuPage));
            Routing.RegisterRoute(nameof(AmmoManuPage), typeof(AmmoManuPage));
            Routing.RegisterRoute(nameof(FirearmManuPage), typeof(FirearmManuPage));
            Routing.RegisterRoute(nameof(ManufacturerAddOrEditPage), typeof(ManufacturerAddOrEditPage));

            Routing.RegisterRoute(nameof(SightsPage), typeof(SightsPage));
            Routing.RegisterRoute(nameof(SightAddOrEditPage), typeof(SightAddOrEditPage));

            Routing.RegisterRoute(nameof(ReticlesPage), typeof(ReticlesPage));
            Routing.RegisterRoute(nameof(ReticleAddOrEditPage), typeof(ReticleAddOrEditPage));

            Routing.RegisterRoute(nameof(SightClickSettingsPage), typeof(SightClickSettingsPage));
            Routing.RegisterRoute(nameof(SightClickSettingAddOrEditPage), typeof(SightClickSettingAddOrEditPage));

            Routing.RegisterRoute(nameof(FirearmsPage), typeof(FirearmsPage));
            Routing.RegisterRoute(nameof(FirearmAddOrEditPage), typeof(FirearmAddOrEditPage));

            Routing.RegisterRoute(nameof(RecordsPage), typeof(RecordsPage));
            Routing.RegisterRoute(nameof(RecordDetailsPage), typeof(RecordDetailsPage));

            Routing.RegisterRoute(nameof(AmmunitionsPage), typeof(AmmunitionsPage));
            Routing.RegisterRoute(nameof(AmmunitionAddOrEditPage), typeof(AmmunitionAddOrEditPage));

            Routing.RegisterRoute(nameof(BulletsPage), typeof(BulletsPage));
            Routing.RegisterRoute(nameof(BulletAddOrEditPage), typeof(BulletAddOrEditPage));

            Routing.RegisterRoute(nameof(MuzzleVelocitiesPage), typeof(MuzzleVelocitiesPage));
            Routing.RegisterRoute(nameof(MuzzleVelocityAddOrEditPage), typeof(MuzzleVelocityAddOrEditPage));

            Routing.RegisterRoute(nameof(ImportExportPage), typeof(ImportExportPage));
        }
    }
}