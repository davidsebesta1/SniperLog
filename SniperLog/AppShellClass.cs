using SniperLog.Pages;
using SniperLog.ViewModels;

namespace SniperLog
{
    public class AppShellClass : Shell
    {
        public AppShellClass()
        {
            FlyoutBehavior = FlyoutBehavior.Flyout;
            SetTabBarIsVisible(this, false);

            MainPage mainPage = new MainPage();
            Items.Add(new ShellContent()
            {
                Title = nameof(MainPage),
                ContentTemplate = new DataTemplate(() => mainPage),
                Route = nameof(mainPage)
            });

            ShootingRangesPage shootingRangesPage = new ShootingRangesPage(MauiProgram.App.Services.GetService<ShootingRangeViewModel>());
            Items.Add(new ShellContent()
            {
                Title = shootingRangesPage.Title,
                ContentTemplate = new DataTemplate(() => shootingRangesPage),
                Route = nameof(shootingRangesPage)
            });

            FlyoutItem tabbar = new FlyoutItem() { Title = "Tabbar", Route = "Tab" };
            Items.Add(tabbar);
        }
    }
}