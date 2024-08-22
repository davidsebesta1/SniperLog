using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Manufacturers
{
    [QueryProperty(nameof(Manufacturer), "Manufacturer")]
    [QueryProperty(nameof(ManufacturerType), "ManuType")]
    public partial class ManufacturerAddOrEditPageViewModel : BaseViewModel
    {
        private readonly ValidatorService _validatorService;
        private readonly DataCacherService<Country> _countryService;

        public string HeadlineText => Manufacturer != null ? "Edit manufacturer" : "New manufacturer";
        public string CreateOrEditButtonText => Manufacturer != null ? "Edit" : "Create";

        [ObservableProperty]
        private ObservableCollection<Country> _countries;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private Manufacturer _manufacturer;

        [ObservableProperty]
        private ManufacturerType _manufacturerType;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private Country _country;

        public ManufacturerAddOrEditPageViewModel(ValidatorService validatorService, DataCacherService<Country> countryCacher) : base()
        {
            _validatorService = validatorService;
            _countryService = countryCacher;
        }

        partial void OnManufacturerChanged(Manufacturer value)
        {
            Name = value?.Name ?? string.Empty;
            Country = value?.ReferencedCountry;
        }

        [RelayCommand]
        private async Task RefreshCountries()
        {
            Countries = await _countryService.GetAll();
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateManufacturer()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            if (Manufacturer == null)
            {
                Manufacturer = new Manufacturer(Country.ID, ManufacturerType.ID, Name);
            }
            else
            {
                Manufacturer.Country_ID = Country.ID;
                Manufacturer.Name = Name;
            }

            await Manufacturer.SaveAsync();

            await Shell.Current.GoToAsync("..");
        }
    }
}