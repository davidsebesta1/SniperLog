using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Firearms
{
    [QueryProperty(nameof(Firearm), "Firearm")]
    public partial class FirearmAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => Firearm != null ? "Edit firearm" : "New firearm";
        public string CreateOrEditButtonText => Firearm != null ? "Edit" : "Create";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private Firearm _firearm;

        #region Entry properties

        [ObservableProperty]
        private FirearmType _firearmType;

        [ObservableProperty]
        private Manufacturer _manufacturer;

        [ObservableProperty]
        private FirearmCaliber _firearmCaliber;

        [ObservableProperty]
        private FirearmSight _firearmSight;

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string? _model;

        [ObservableProperty]
        public string? _serialNumber;

        [ObservableProperty]
        public double? _totalLengthMm;

        [ObservableProperty]
        public double? _barrelLengthInch;

        [ObservableProperty]
        public string? _rateOfTwist;

        [ObservableProperty]
        public double? _weight;

        [ObservableProperty]
        public bool? _handednessRight;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private double? _sightOffsetCm;

        [ObservableProperty]
        private ObservableCollection<FirearmType> _firearmTypes;

        [ObservableProperty]
        private ObservableCollection<Manufacturer> _manufacturers;

        [ObservableProperty]
        private ObservableCollection<FirearmCaliber> _calibers;

        [ObservableProperty]
        private ObservableCollection<FirearmSight> _sights;

        #endregion

        private readonly ValidatorService _validatorService;

        private readonly DataCacherService<FirearmType> _firearmTypeCacher;
        private readonly DataCacherService<Manufacturer> _manufacturerCacher;
        private readonly DataCacherService<FirearmCaliber> _caliberCacher;
        private readonly DataCacherService<FirearmSight> _firearmSightCacher;

        public FirearmAddOrEditPageViewModel(ValidatorService validatorService, DataCacherService<FirearmType> firearmTypeCacher, DataCacherService<Manufacturer> manufacturerCacher, DataCacherService<FirearmCaliber> caliberCacher, DataCacherService<FirearmSight> firearmSightCacher)
        {
            _validatorService = validatorService;
            _firearmTypeCacher = firearmTypeCacher;
            _manufacturerCacher = manufacturerCacher;
            _caliberCacher = caliberCacher;
            _firearmSightCacher = firearmSightCacher;
        }

        partial void OnFirearmChanged(Firearm value)
        {
            FirearmType = value?.ReferencedFirearmType;
            Manufacturer = value?.ReferencedManufacturer;
            FirearmCaliber = value?.ReferencedFirearmCaliber;
            FirearmSight = value?.ReferencedFirearmSight;

            Name = value?.Name ?? string.Empty;
            Model = value?.Model ?? string.Empty;
            SerialNumber = value?.SerialNumber ?? string.Empty;
            TotalLengthMm = value?.TotalLengthMm;
            BarrelLengthInch = value?.BarrelLengthInch;
            RateOfTwist = value?.RateOfTwist;
            Weight = value?.Weight;
            HandednessRight = !value?.HandednessForLeft;
            Notes = value?.NotesText;
            SightOffsetCm = value?.SightHeightCm;
        }

        [RelayCommand]
        private async Task RefeshPickers()
        {
            FirearmTypes = await _firearmTypeCacher.GetAll();
            Manufacturers = await _manufacturerCacher.GetAll();
            Calibers = await _caliberCacher.GetAll();
            Sights = await _firearmSightCacher.GetAll();
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

            if (Firearm == null)
            {
                string notes = Notes;
                Firearm = new Firearm(FirearmType.ID, Manufacturer.ID, FirearmCaliber.ID, FirearmSight.ID, Name, Model, SerialNumber, TotalLengthMm, BarrelLengthInch, RateOfTwist, Weight, !HandednessRight, SightOffsetCm);

                if (!string.IsNullOrEmpty(notes))
                    await Firearm.SaveNotesAsync(notes);
                
            }
            else
            {
                Firearm.FirearmType_ID = FirearmType.ID;
                Firearm.Manufacturer_ID = Manufacturer.ID;
                Firearm.Caliber_ID = FirearmCaliber.ID;
                Firearm.FirearmSight_ID = FirearmSight.ID;
                Firearm.Name = Name;
                Firearm.Model = Model;
                Firearm.SerialNumber = SerialNumber;
                Firearm.TotalLengthMm = TotalLengthMm;
                Firearm.BarrelLengthInch = BarrelLengthInch;
                Firearm.RateOfTwist = RateOfTwist;
                Firearm.Weight = Weight;
                Firearm.HandednessForLeft = !HandednessRight;
                Firearm.SightHeightCm = SightOffsetCm;

                if (Notes != Firearm.NotesText)
                    await Firearm.SaveNotesAsync(Notes);
                
            }

            await Firearm.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}
