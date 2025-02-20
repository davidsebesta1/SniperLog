using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Firearms
{
    [QueryProperty(nameof(Ammunition), "Ammunition")]
    public partial class AmmunitionAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => Ammunition != null ? "Edit ammunition" : "New ammunition";
        public string CreateOrEditButtonText => Ammunition != null ? "Edit" : "Create";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private Ammunition _ammunition;

        [ObservableProperty]
        private Bullet _bullet;

        [ObservableProperty]
        private double _totalLengthMm;

        [ObservableProperty]
        private double _gunpowderAmountGrams;

        private ValidatorService _validatorService;

        private DataCacherService<Bullet> _bulletService;

        [ObservableProperty]
        private ObservableCollection<Bullet> _bullets;

        public AmmunitionAddOrEditPageViewModel(ValidatorService validatorService, DataCacherService<Bullet> bulletService)
        {
            _validatorService = validatorService;
            _bulletService = bulletService;
        }

        async partial void OnAmmunitionChanged(Ammunition value)
        {
            Bullet = value?.ReferencedBullet;

            TotalLengthMm = value?.TotalLengthMm ?? 0;
            GunpowderAmountGrams = value?.GunpowderAmountGrams ?? 0;

        }

        [RelayCommand]
        private async Task RefeshPickers()
        {
            Bullets = await _bulletService.GetAll();
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

            if (Ammunition == null)
            {
                Ammunition = new Ammunition(Bullet.ID, TotalLengthMm, GunpowderAmountGrams);
            }
            else
            {
                Ammunition.Bullet_ID = Bullet.ID;
                Ammunition.TotalLengthMm = TotalLengthMm;
                Ammunition.GunpowderAmountGrams = GunpowderAmountGrams;
            }

            await Ammunition.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}
