namespace SniperLog.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        Test.Text = VersionTracking.Default.CurrentVersion;
    }
}