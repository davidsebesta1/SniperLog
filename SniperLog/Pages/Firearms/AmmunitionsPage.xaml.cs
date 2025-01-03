using SniperLog.ViewModels.Firearms;

namespace SniperLog.Pages.Firearms
{
    public partial class AmmunitionsPage : ContentPage
    {
        public AmmunitionsPage(AmmunitionsPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as AmmunitionsPageViewModel).RefreshCommand.ExecuteAsync(null);
        }
    }
}