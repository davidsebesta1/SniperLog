using CommunityToolkit.Mvvm.Input;
using SniperLog.Extensions;
using SniperLog.Extensions.WrapperClasses;
using System.Globalization;

namespace SniperLog.ViewModels.SRanges
{
    [QueryProperty(nameof(Range), "ShootingRange")]
    public partial class SRangesAddOrEditPageViewModel : BaseViewModel
    {
        private readonly ValidatorService _validatorService;

        public string HeadlineText => Range != null ? "Edit shooting range" : "New shooting range";
        public string CreateOrEditButtonText => Range != null ? "Edit" : "Create";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
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
        private DrawableImagePaths _backgroundImagePath;

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
            BackgroundImagePath = value?.BackgroundImgPathFull;
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateSRange()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            if (Range == null)
            {
                Range = new ShootingRange(Name, Address, ParseExtensions.ParseOrNullDouble(Lat), ParseExtensions.ParseOrNullDouble(Lon), false);

                if (!string.IsNullOrEmpty(BackgroundImagePath))
                {
                    using (FileStream reader = File.OpenRead(BackgroundImagePath))
                    {
                        await Range.SaveImageAsync(reader);
                    }
                }
            }
            else
            {
                Range.Name = Name;
                Range.Address = Address;
                Range.Latitude = ParseExtensions.ParseOrNullDouble(Lat);
                Range.Longitude = ParseExtensions.ParseOrNullDouble(Lon);

                if (!string.IsNullOrEmpty(BackgroundImagePath) && BackgroundImagePath != Range.BackgroundImgPathFull)
                {
                    using (FileStream reader = File.OpenRead(BackgroundImagePath))
                    {
                        await Range.SaveImageAsync(reader);
                    }
                }
            }


            await Range.SaveAsync();

            await Shell.Current.GoToAsync("..");
        }
    }
}