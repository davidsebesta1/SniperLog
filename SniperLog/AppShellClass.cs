using SniperLog.Pages;

namespace SniperLog
{
    public class AppShellClass : Shell
    {
        public AppShellClass()
        {
            FlyoutBehavior = FlyoutBehavior.Flyout;
            SetTabBarIsVisible(this, false);
            Items.Add(new ShellContent()
            {
                Title = "MainPage",
                ContentTemplate = new DataTemplate(() => new MainPage()),
                Route = "MainPage"
            });

            Items.Add(new ShellContent()
            {
                Title = "Shooting Ranges",
                ContentTemplate = new DataTemplate(() => new ShootingRangesPage()),
                Route = "ShootingRangesPage"
            });

            var tabbar = new FlyoutItem() { Title = "Tabbar", Route = "Tab" };
            Items.Add(tabbar);
        }
    }
}