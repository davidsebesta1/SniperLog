using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Manufacturers.FirearmManufacturers
{
    public partial class FirearmManuPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<Manufacturer> _manufacturers;

        private readonly DataCacherService<Manufacturer> _manCacher;
        private readonly DataCacherService<ManufacturerType> _manTypeCacher;

        public FirearmManuPageViewModel(DataCacherService<Manufacturer> manufacturersCacher, DataCacherService<ManufacturerType> manTypeCacher) : base()
        {
            PageTitle = "Firearm\nManufacturers";
            _manCacher = manufacturersCacher;
            _manTypeCacher = manTypeCacher;
        }

        [RelayCommand]
        private async Task RefreshManufacturers()
        {
            int id = (await _manTypeCacher.GetFirstBy(n => n.Name == "Sight")).ID;
            Manufacturers = await _manCacher.GetAllBy(n => n.ManufacturerType_ID == id);
        }

        [RelayCommand]
        private async Task SearchManufacturers(string text)
        {
            int id = (await _manTypeCacher.GetFirstBy(n => n.Name == "Sight")).ID;
            Manufacturers = await _manCacher.GetAllBy(n => n.ManufacturerType_ID == id && (string.IsNullOrEmpty(text) || n.Name.Contains(text)));
        }

        [RelayCommand]
        private async Task ReturnBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
