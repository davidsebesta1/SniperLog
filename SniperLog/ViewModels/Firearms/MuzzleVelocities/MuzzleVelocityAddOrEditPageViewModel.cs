using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Firearms.MuzzleVelocities
{
    [QueryProperty(nameof(MuzzleVelocity), "MuzzleVelocity")]
    [QueryProperty(nameof(Firearm), "Firearm")]
    public partial class MuzzleVelocityAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => MuzzleVelocity != null ? "Edit muzzle velocity" : "New muzzle velocity";
        public string CreateOrEditButtonText => MuzzleVelocity != null ? "Edit" : "Create";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private MuzzleVelocity _muzzleVelocity;

        [ObservableProperty]
        private Firearm _firearm;

        [ObservableProperty]
        private Ammunition _selectedAmmo;

        [ObservableProperty]
        private double _muzzleVelocityData;

        [ObservableProperty]
        private ObservableCollection<Ammunition> _ammunition;

        private ValidatorService _validatorService;

        private DataCacherService<Ammunition> _ammunitionService;

        public MuzzleVelocityAddOrEditPageViewModel(ValidatorService validatorService, DataCacherService<Ammunition> bulletService)
        {
            _validatorService = validatorService;
            _ammunitionService = bulletService;
        }

        partial void OnMuzzleVelocityChanged(MuzzleVelocity value)
        {
            MuzzleVelocityData = value?.VelocityMS ?? 0d;
            SelectedAmmo = value?.ReferencedAmmunition ?? null;
        }

        [RelayCommand]
        private async Task RefreshPickers()
        {
            Ammunition = await _ammunitionService.GetAll();
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Create()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            if(MuzzleVelocity != null)
            {
                MuzzleVelocity.Ammo_ID = SelectedAmmo.ID;
                MuzzleVelocity.VelocityMS = MuzzleVelocityData;
            }
            else
            {
                MuzzleVelocity = new MuzzleVelocity(SelectedAmmo.ID, Firearm.ID, MuzzleVelocityData);
            }

            await MuzzleVelocity.SaveAsync();

            await Shell.Current.GoToAsync("..");
        }
    }
}
