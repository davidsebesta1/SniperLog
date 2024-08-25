using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace SniperLog.ViewModels.Sights.ClickSettings
{
    [QueryProperty(nameof(Setting), "Setting")]
    [QueryProperty(nameof(Sight), "Sight")]
    public partial class SightClickSettingAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => Setting != null ? "Edit click setting" : "New click setting";
        public string CreateOrEditButtonText => Setting != null ? "Edit" : "Create";

        [ObservableProperty]
        private FirearmSight _sight;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private FirearmSightSetting _setting;

        [ObservableProperty]
        private string _distanceString;

        [ObservableProperty]
        private string _upClickString;

        [ObservableProperty]
        private string _windClicksString;

        private readonly ValidatorService _validatorService;

        public SightClickSettingAddOrEditPageViewModel(ValidatorService validatorService)
        {
            _validatorService = validatorService;
        }

        partial void OnSettingChanged(FirearmSightSetting value)
        {
            DistanceString = Setting?.Distance.ToString() ?? string.Empty;
            UpClickString = Setting?.ElevationValue.ToString() ?? string.Empty;
            WindClicksString = Setting?.WindageValue.ToString() ?? string.Empty;
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateSightSetting()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            if (Setting == null)
            {
                Setting = new FirearmSightSetting(Sight.ID, int.Parse(DistanceString, CultureInfo.InvariantCulture), int.Parse(UpClickString, CultureInfo.InvariantCulture), int.Parse(WindClicksString, CultureInfo.InvariantCulture));
            } else
            {
                Setting.Distance = int.Parse(DistanceString, CultureInfo.InvariantCulture);
                Setting.ElevationValue = int.Parse(UpClickString, CultureInfo.InvariantCulture);
                Setting.WindageValue = int.Parse(WindClicksString, CultureInfo.InvariantCulture);
            }

            await Setting.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}