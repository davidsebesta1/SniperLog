using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SniperLog.ViewModels.Sights
{
    [QueryProperty(nameof(Sight), "Sight")]
    public partial class SightAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => Sight != null ? "Edit firearm sight" : "New firearm sight";
        public string CreateOrEditButtonText => Sight != null ? "Edit" : "Create";
        public string ClickTypeName => ClickType == 0 ? "MOA" : "MRAD";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private FirearmSight _sight;

        [ObservableProperty]
        private int _clickType;

        [ObservableProperty]
        private Manufacturer _manufacturer;

        [ObservableProperty]
        private SightReticle _reticle;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _oneClickValue;

        [ObservableProperty]
        private ObservableCollection<Manufacturer> _manufacturers;

        [ObservableProperty]
        private ObservableCollection<SightClickType> _sightClickTypes;

        [ObservableProperty]
        private ObservableCollection<SightReticle> _sightReticles;

        private readonly DataCacherService<Manufacturer> _manufacturerService;
        private readonly DataCacherService<SightClickType> _sightClickService;
        private readonly DataCacherService<SightReticle> _sightReticleService;
        private readonly ValidatorService _validatorService;

        public SightAddOrEditPageViewModel(DataCacherService<Manufacturer> manufacturerService, DataCacherService<SightClickType> sightClickService, DataCacherService<SightReticle> sightReticlesService, ValidatorService validatorService)
        {
            _manufacturerService = manufacturerService;
            _sightClickService = sightClickService;
            _sightReticleService = sightReticlesService;
            _validatorService = validatorService;
        }

        partial void OnSightChanged(FirearmSight value)
        {
            ClickType = value?.ReferencedSightClickType.ClickTypeName == "MRAD" ? 1 : 0;
            Manufacturer = value?.ReferencedManufacturer ?? null;
            Reticle = value?.ReferencedSightReticle ?? null;
            Name = value?.Name ?? string.Empty;
            OneClickValue = value?.OneClickValue.ToString() ?? string.Empty;
        }

        [RelayCommand]
        private async Task RefreshManufacturers()
        {
            Manufacturers = await _manufacturerService.GetAll();
        }

        [RelayCommand]
        private async Task RefreshClickTypes()
        {
            SightClickTypes = await _sightClickService.GetAll();
        }

        [RelayCommand]
        private async Task RefreshSightReticles()
        {
            SightReticles = await _sightReticleService.GetAll();
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateFirearmSight()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            SightClickType sightClickType = SightClickTypes.FirstOrDefault(n => n.ClickTypeName == ClickTypeName);

            if (sightClickType == null)
            {
                await Shell.Current.DisplayAlert("Error", $"Unable to find sight click type of name '{ClickTypeName}'", "Okay");
                return;
            }

            if (Sight == null)
            {
                Sight = new FirearmSight(sightClickType.ID, Manufacturer.ID, Reticle.ID, Name, double.Parse(OneClickValue, CultureInfo.InvariantCulture));
            }
            else
            {
                Sight.ClickType_ID = sightClickType.ID;
                Sight.Manufacturer_ID = Manufacturer.ID;
                Sight.SightReticle_ID = Reticle.ID;
                Sight.Name = Name;
                Sight.OneClickValue = double.Parse(OneClickValue, CultureInfo.InvariantCulture);
            }

            await Sight.SaveAsync();

            await Shell.Current.GoToAsync("..");
        }
    }
}
