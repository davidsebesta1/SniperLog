using CommunityToolkit.Mvvm.Input;

namespace SniperLog.ViewModels.Manufacturers
{
    public partial class ManufacturersPageViewModel : BaseViewModel
    {
        public ManufacturersPageViewModel() : base()
        {
            PageTitle = "Manufacturers";
        }

        [RelayCommand]
        private async Task GoToFirearmManufacturers()
        {
            await Shell.Current.GoToAsync("Manufacturers/FirearmManufacturers");
        }

        [RelayCommand]
        private async Task GoToSightManufacturers()
        {
            await Shell.Current.GoToAsync("Manufacturers/SightManufacturers");
        }
    }
}
