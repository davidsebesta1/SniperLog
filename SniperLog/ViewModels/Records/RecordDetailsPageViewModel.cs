using CommunityToolkit.Mvvm.Input;
using SniperLog.Extensions.WrapperClasses;

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
        private DrawableImagePaths _img;

        public RecordDetailsPageViewModel(ValidatorService validatorService)
        {
            _validatorService = validatorService;
        }

        async partial void OnRecordChanged(ShootingRecord value)
        {
            try
            {
                PageTitle = value != null ? $"{Record.Date.ToString("d")}\n{Record.Date.ToString("t")}" : string.Empty;

                ElevationClicks = value?.ElevationClicksOffset ?? 0;
                WindageClicks = value?.WindageClicksOffset ?? 0;
                DistanceMeters = value?.Distance ?? 0;

                Img = value != null ? new DrawableImagePaths((await value.GetImagesAsync()).ElementAtOrDefault(0)?.BackgroundImgPathFull) : null;
                Notes = value?.NotesText ?? string.Empty;
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Err", e.ToString(), "okay");
            }
        }

        [RelayCommand]
        private async Task ReturnBack()
        {
            Record = null;
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
            try
            {
                Record.ElevationClicksOffset = ElevationClicks;
                Record.WindageClicksOffset = WindageClicks;
                Record.Distance = DistanceMeters;

                if (Record.NotesText != Notes)
                {
                    await Record.SaveNotesAsync(Notes);
                }

                ShootingRecordImage? image = (await Record.GetImagesAsync()).ElementAtOrDefault(0);

                if (image == null && !string.IsNullOrEmpty(Img.ImagePath))
                {
                    image = new ShootingRecordImage(Record.ID);
                    await image.SaveAsync();
                }

                if (image != null && (image.ImageSavePath != Img.ImagePath || File.Exists(Img.OverDrawPath)))
                {
                    await image.SaveImageAsync(Img);
                }

            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Err", e.ToString(), "Ok");
            }
            finally
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
