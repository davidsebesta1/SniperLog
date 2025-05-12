using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.Manufacturers.FirearmManufacturers;

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
            await Shell.Current.GoToAsync(nameof(FirearmManuPage));
        }

        [RelayCommand]
        private async Task GoToSightManufacturers()
        {
            await Shell.Current.GoToAsync(nameof(SightManuPage));
        }

        [RelayCommand]
        private async Task GoToBulletManufacturers()
        {
            await Shell.Current.GoToAsync(nameof(BulletManuPage));
        }

        [RelayCommand]
        private async Task GoToAmmunitionManufacturers()
        {
            await Shell.Current.GoToAsync(nameof(AmmoManuPage));
        }
    }
}
