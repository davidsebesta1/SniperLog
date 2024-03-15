using SniperLog.Pages.ShootingRanges;

namespace SniperLog;

public partial class ShellClass : Shell
{
	public ShellClass()
	{
		InitializeComponent();
        SetTabBarIsVisible(this, false);

        #region Routes

        Routing.RegisterRoute(nameof(ShootingRangeDetailsPage), typeof(ShootingRangeDetailsPage));
        Routing.RegisterRoute(nameof(ShootingRangeSubRangesPage), typeof(ShootingRangeSubRangesPage));

        #endregion
    }
}