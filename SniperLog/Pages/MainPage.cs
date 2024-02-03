namespace SniperLog.Pages;

public class MainPage : BasePage
{
    public MainPage()
    {
        Build();
    }

    public override void Build()
    {
        Content = new VerticalStackLayout
        {
            Children = {
                new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to Sniper Log"
                }
            }
        };
    }
}