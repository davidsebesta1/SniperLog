using SniperLog.ViewModels.Sights;

namespace SniperLog.Pages.Sights
{
    public partial class SightsPage : ContentPage
    {
        public SightsPage(SightsPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as SightsPageViewModel).RefreshSightsCommand.ExecuteAsync(null);
        }
    }
}