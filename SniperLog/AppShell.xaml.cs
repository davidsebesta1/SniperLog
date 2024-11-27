using SniperLog.Pages.Firearms;
using SniperLog.Pages.Manufacturers;
using SniperLog.Pages.Manufacturers.FirearmManufacturers;
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

            Routing.RegisterRoute("AddOrEditRange", typeof(SRangesAddOrEditPage));
            Routing.RegisterRoute("RangeDetails", typeof(SRangeDetailsPage));
            Routing.RegisterRoute("Subranges", typeof(SubRangesPage));
            Routing.RegisterRoute("AddOrEditSubrange", typeof(SubRangeAddOrEditPage));

            Routing.RegisterRoute("Manufacturers", typeof(ManufacturersPage));
            Routing.RegisterRoute("Manufacturers/FirearmManufacturers", typeof(FirearmManuPage));
            Routing.RegisterRoute("Manufacturers/SightManufacturers", typeof(SightManuPage));
            Routing.RegisterRoute("Manufacturers/AddOrEdit", typeof(ManufacturerAddOrEditPage));

            Routing.RegisterRoute("Sights", typeof(SightsPage));
            Routing.RegisterRoute("Sights/AddOrEdit", typeof(SightAddOrEditPage));

            Routing.RegisterRoute("Reticles", typeof(ReticlesPage));
            Routing.RegisterRoute("Reticles/AddOrEdit", typeof(ReticleAddOrEditPage));

            Routing.RegisterRoute("SightClickSettings", typeof(SightClickSettingsPage));
            Routing.RegisterRoute("SightClickSettings/AddOrEdit", typeof(SightClickSettingAddOrEditPage));

            Routing.RegisterRoute("Firearms", typeof(FirearmsPage));
            Routing.RegisterRoute("Firearms/AddOrEdit", typeof(FirearmAddOrEditPage));

            Routing.RegisterRoute("Records", typeof(RecordsPage));
            Routing.RegisterRoute("Records/RecordDetails", typeof(RecordDetailsPage));
        }
    }
}