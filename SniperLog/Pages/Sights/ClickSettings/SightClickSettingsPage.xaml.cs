using SniperLog.ViewModels.Sights.ClickSettings;

namespace SniperLog.Pages.Sights.ClickSettings
{
    public partial class SightClickSettingsPage : ContentPage
    {
        public SightClickSettingsPage(SightClickSettingsPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await (BindingContext as SightClickSettingsPageViewModel).RefreshClickSettingsCommand.ExecuteAsync(null);
        }
    }
}