using SniperLog.ViewModels.Manufacturers.BulletManufacturers;

namespace SniperLog.Pages.Manufacturers.FirearmManufacturers
{
    public partial class BulletManuPage : ContentPage
    {
        public BulletManuPage(BulletManuPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as BulletManuPageViewModel).RefreshManufacturersCommand.ExecuteAsync(null);
        }
    }
}