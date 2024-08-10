using SniperLog.ViewModels.Manufacturers;

namespace SniperLog.Pages.Manufacturers
{
    public partial class ManufacturersPage : ContentPage
    {
        public ManufacturersPage(ManufacturersPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}