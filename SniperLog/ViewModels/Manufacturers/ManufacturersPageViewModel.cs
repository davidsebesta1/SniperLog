using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
