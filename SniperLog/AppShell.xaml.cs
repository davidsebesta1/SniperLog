using SniperLog.Pages.Manufacturers;
using SniperLog.Pages.Manufacturers.FirearmManufacturers;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Pages.ShootingRanges.Subranges;

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
        }
    }
}