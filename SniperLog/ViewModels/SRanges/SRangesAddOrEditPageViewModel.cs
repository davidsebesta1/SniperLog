using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace SniperLog.ViewModels.SRanges
{
    [QueryProperty(nameof(Range), "ShootingRange")]
    public partial class SRangesAddOrEditPageViewModel : BaseViewModel
    {
        private readonly ValidatorService _validatorService;

        [ObservableProperty]
        private ShootingRange _range;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _address;

        [ObservableProperty]
        private string _lat;

        [ObservableProperty]
        private string _lon;

        [ObservableProperty]
        private int _locType = 1;

        [ObservableProperty]
        private string _backgroundImagePath;

        public SRangesAddOrEditPageViewModel(ValidatorService validator) : base()
        {
            _validatorService = validator;
        }

        partial void OnRangeChanged(ShootingRange value)
        {
            Name = value?.Name ?? string.Empty;
            Address = value?.Address ?? string.Empty;
            Lat = value?.Latitude.ToString() ?? string.Empty;
            Lon = value?.Longitude.ToString() ?? string.Empty;
            LocType = 1;
            BackgroundImagePath = value?.BackgroundImgPath ?? string.Empty;
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateSRange()
        {
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            ShootingRange range = new ShootingRange(Name, Address, double.Parse(Lat, CultureInfo.InvariantCulture), double.Parse(Lon, CultureInfo.InvariantCulture), false, BackgroundImagePath);

            using (FileStream reader = File.OpenRead(BackgroundImagePath))
            {
                await range.SaveImageAsync(reader);
            }

            await range.SaveAsync();

            await Shell.Current.GoToAsync("..");
        }
    }
}