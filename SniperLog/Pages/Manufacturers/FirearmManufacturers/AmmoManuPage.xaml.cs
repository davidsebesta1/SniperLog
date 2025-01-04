using SniperLog.ViewModels.Manufacturers.AmmunitionManufacturers;

namespace SniperLog.Pages.Manufacturers.FirearmManufacturers
{
    public partial class AmmoManuPage : ContentPage
    {
        public AmmoManuPage(AmmoManuPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as AmmoManuPageViewModel).RefreshManufacturersCommand.ExecuteAsync(null);
        }
    }
}