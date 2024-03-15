using SniperLog.Models;
using SniperLog.Pages;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Services;
using SniperLog.ViewModels;

namespace SniperLog
{
    public class AppShellClass : Shell
    {
        public AppShellClass()
        {
            FlyoutBehavior = FlyoutBehavior.Flyout;
            SetTabBarIsVisible(this, false);

            #region Pages
            MainPage mainPage = new MainPage();
            Items.Add(new ShellContent()
            {
                Title = nameof(MainPage),
                ContentTemplate = new DataTemplate(() => mainPage),
                Route = nameof(mainPage)
            });

            ShootingRangesPage shootingRangesPage = new ShootingRangesPage(ServicesHelper.GetService<ShootingRangeViewModel>());

            Items.Add(new ShellContent()
            {
                Title = shootingRangesPage.Title,
                ContentTemplate = new DataTemplate(() => shootingRangesPage),
                Route = nameof(shootingRangesPage),
            });

            FlyoutItem flyoutItems = new FlyoutItem() { Title = "FlyoutBar", Route = "Flyout", FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems };
            Items.Add(flyoutItems);

            #endregion

            #region Routes

            Routing.RegisterRoute(nameof(ShootingRangeDetailsPage), typeof(ShootingRangeDetailsPage));
            Routing.RegisterRoute(nameof(ShootingRangeSubRangesPage), typeof(ShootingRangeSubRangesPage));

            #endregion
        }
    }
}
