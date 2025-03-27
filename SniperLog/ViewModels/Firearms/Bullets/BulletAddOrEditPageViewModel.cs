using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Firearms.Bullets
{
    [QueryProperty(nameof(Bullet), "Bullet")]
    public partial class BulletAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => Bullet != null ? "Edit bullet" : "New bullet";
        public string CreateOrEditButtonText => Bullet != null ? "Edit" : "Create";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private Bullet _bullet;

        [ObservableProperty]
        private FirearmCaliber _caliber;

        [ObservableProperty]
        private Manufacturer _manufacturer;

        [ObservableProperty]
        private double _weightGrams;

        [ObservableProperty]
        private double _bulletDiameter;

        [ObservableProperty]
        private double _bulletLength;

        [ObservableProperty]
        private double? _bc1;

        [ObservableProperty]
        private double? _bc7;

        private ValidatorService _validatorService;

        private DataCacherService<FirearmCaliber> _caliberService;
        private DataCacherService<Manufacturer> _manufacturerService;
        private DataCacherService<ManufacturerType> _manufacturerTypeService;

        [ObservableProperty]
        private ObservableCollection<FirearmCaliber> _calibers;

        [ObservableProperty]
        private ObservableCollection<Manufacturer> _manufacturers;

        public BulletAddOrEditPageViewModel(ValidatorService validatorService, DataCacherService<FirearmCaliber> caliberService, DataCacherService<Manufacturer> manuService, DataCacherService<ManufacturerType> mtService)
        {
            _validatorService = validatorService;
            _caliberService = caliberService;
            _manufacturerService = manuService;
            _manufacturerTypeService = mtService;
        }

        partial void OnBulletChanged(Bullet value)
        {
            Caliber = value?.ReferencedFirearmCaliber ?? null;
            Manufacturer = value?.ReferencedManufacturer ?? null;
            WeightGrams = value?.WeightGrams ?? 0;
            BulletDiameter = value?.BulletDiameter ?? 0;
            BulletLength = value?.BulletLength ?? 0;
            Bc1 = value?.BCG1;
            Bc7 = value?.BCG7;
        }

        [RelayCommand]
        private async Task RefeshPickers()
        {
            Calibers = await _caliberService.GetAll();

            ManufacturerType bulletManufacturerType = await _manufacturerTypeService.GetFirstBy(n => n.Name == "Bullet");
            Manufacturers = await _manufacturerService.GetAllBy(n => n.ReferencedManufacturerType == bulletManufacturerType);
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

            if (Bullet == null)
            {
                Bullet = new Bullet(Caliber.ID, Manufacturer.ID, WeightGrams, BulletDiameter, BulletLength, Bc1, Bc7);
            }
            else
            {
                Bullet.WeightGrams = WeightGrams;
                Bullet.Caliber_ID = Caliber.ID;
                Bullet.Manufacturer_ID = Manufacturer.ID;
                Bullet.WeightGrams = WeightGrams;
                Bullet.BulletDiameter = BulletDiameter;
                Bullet.BulletLength = BulletLength;
                Bullet.BCG1 = Bc1;
                Bullet.BCG7 = Bc7;
            }

            await Bullet.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}
