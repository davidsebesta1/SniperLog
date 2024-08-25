using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Manufacturers
{
    public abstract partial class ManuPageViewModelBase : BaseViewModel
    {
        [ObservableProperty]
        protected ObservableCollection<Manufacturer> _manufacturers;

        protected readonly DataCacherService<Manufacturer> _manCacher;
        protected readonly DataCacherService<ManufacturerType> _manTypeCacher;

        /// <summary>
        /// Internal name of the manufacturer type, eg "Firearm" or "Sight"
        /// </summary>
        public abstract string ManufacturerTypeNameInternal { get; }

        public ManuPageViewModelBase(DataCacherService<Manufacturer> manufacturersCacher, DataCacherService<ManufacturerType> manTypeCacher) : base()
        {
            _manCacher = manufacturersCacher;
            _manTypeCacher = manTypeCacher;
        }

        [RelayCommand]
        protected async Task RefreshManufacturers()
        {
            int id = (await _manTypeCacher.GetFirstBy(n => n.Name == ManufacturerTypeNameInternal)).ID;
            Manufacturers = await _manCacher.GetAllBy(n => n.ManufacturerType_ID == id);
        }

        [RelayCommand]
        protected async Task SearchManufacturers(string text)
        {
            int id = (await _manTypeCacher.GetFirstBy(n => n.Name == ManufacturerTypeNameInternal)).ID;
            Manufacturers = await _manCacher.GetAllBy(n => n.ManufacturerType_ID == id && (string.IsNullOrEmpty(text) || n.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase)));
        }

        [RelayCommand]
        protected async Task ReturnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        protected async Task EditManufacturer(Manufacturer manufacturer)
        {
            await Shell.Current.GoToAsync("Manufacturers/AddOrEdit", new Dictionary<string, object>(2) { { "Manufacturer", manufacturer }, { "ManuType", await _manTypeCacher.GetFirstBy(n => n.Name == ManufacturerTypeNameInternal) } });
        }

        [RelayCommand]
        protected async Task DeleteManufacturer(Manufacturer manufacturer)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {manufacturer.Name}? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await manufacturer.DeleteAsync();
                Manufacturers.Remove(manufacturer);
            }
        }

        [RelayCommand]
        protected async Task CreateNewManufacturer()
        {
            await Shell.Current.GoToAsync("Manufacturers/AddOrEdit", new Dictionary<string, object>(2) { { "Manufacturer", null }, { "ManuType", await _manTypeCacher.GetFirstBy(n => n.Name == ManufacturerTypeNameInternal) } });
        }
    }
}
