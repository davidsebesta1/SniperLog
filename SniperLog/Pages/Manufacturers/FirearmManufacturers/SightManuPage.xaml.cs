using SniperLog.ViewModels.Manufacturers.SightManufacturers;

namespace SniperLog.Pages.Manufacturers.FirearmManufacturers
{
    public partial class SightManuPage : ContentPage
    {
        public SightManuPage(SightManuPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as SightManuPageViewModel).RefreshManufacturersCommand.ExecuteAsync(null);
        }
    }
}