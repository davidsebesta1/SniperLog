using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Records
{
    [QueryProperty(nameof(Record), "Record")]
    public partial class RecordDetailsPageViewModel : BaseViewModel
    {
        private readonly ValidatorService _validatorService;

        [ObservableProperty]
        private ShootingRecord _record;

        [ObservableProperty]
        private int _elevationClicks;

        [ObservableProperty]
        private int _windageClicks;

        [ObservableProperty]
        private int _distanceMeters;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private string _imgPath;

        public RecordDetailsPageViewModel(ValidatorService validatorService)
        {
            _validatorService = validatorService;
        }

        async partial void OnRecordChanged(ShootingRecord value)
        {
            PageTitle = $"{Record.Date.ToString("d")}\n{Record.Date.ToString("t")}";

            ElevationClicks = value.ElevationClicksOffset;
            WindageClicks = value.WindageClicksOffset;
            DistanceMeters = value.Distance;

            ImgPath = (await value.GetImagesAsync()).ElementAtOrDefault(0)?.BackgroundImgPathFull;
            Notes = value.NotesText;
        }

        [RelayCommand]
        private async Task ReturnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Delete()
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete this record? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await Record.DeleteAsync();
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task Edit()
        {
            Record.ElevationClicksOffset = ElevationClicks;
            Record.WindageClicksOffset = WindageClicks;
            Record.Distance = DistanceMeters;

            if (Record.NotesText != Notes)
            {
                await Record.SaveNotesAsync(Notes);
            }
        }
    }
}
