using SniperLog.Pages.ShootingRanges;

namespace SniperLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("AddOrEditRange", typeof(SRangesAddOrEditPage));
            Routing.RegisterRoute("RangeDetails", typeof(SRangeDetailsPage));
        }
    }
}