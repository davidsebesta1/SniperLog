using SniperLog.ViewModels.Manufacturers.FirearmManufacturers;

namespace SniperLog.Pages.Manufacturers.FirearmManufacturers
{
    public partial class FirearmManuPage : ContentPage
    {
        public FirearmManuPage(FirearmManuPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as FirearmManuPageViewModel).RefreshManufacturersCommand.ExecuteAsync(null);
        }
    }
}