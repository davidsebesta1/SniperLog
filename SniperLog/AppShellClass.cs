using SniperLog.Pages;
using SniperLog.Pages.ShootingRanges;
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

            ShootingRangesPage shootingRangesPage = new ShootingRangesPage(MauiProgram.ApplicationInstance.Services.GetService<ShootingRangeViewModel>());
            Items.Add(new ShellContent()
            {
                Title = shootingRangesPage.Title,
                ContentTemplate = new DataTemplate(() => shootingRangesPage),
                Route = nameof(shootingRangesPage)
            });

            FlyoutItem tabbar = new FlyoutItem() { Title = "Tabbar", Route = "Tab" };
            Items.Add(tabbar);

            #endregion

            #region Routes

            Routing.RegisterRoute(nameof(ShootingRangeDetailsPage), typeof(ShootingRangeDetailsPage));

            #endregion
        }
    }
}