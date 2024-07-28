using CommunityToolkit.Mvvm.Input;

namespace SniperLog.ViewModels.SRanges
{
    [QueryProperty(nameof(Range), "ShootingRange")]
    public partial class SRangeDetailsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRange _range;

        public SRangeDetailsPageViewModel() : base()
        {
        }

        [RelayCommand]
        private async Task ReturnToRanges()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task EditRange()
        {
            await Shell.Current.GoToAsync("AddOrEditRange", new Dictionary<string, object>() { { "ShootingRange", Range } });
        }

        [RelayCommand]
        private async Task RefreshWeather()
        {
            await Range.TrySendWeatherRequestMessage();
        }

        [RelayCommand]
        private async Task GoToSubranges()
        {
            //todo
        }

        [RelayCommand]
        private async Task ToggleFavouriteRange()
        {
            Range.IsMarkedAsFavourite = !Range.IsMarkedAsFavourite;
            await Range.SaveAsync();
        }

        [RelayCommand]
        private async Task StartNavigation()
        {
            MapLaunchOptions options = new MapLaunchOptions { Name = Range.Name };

            try
            {
                await Map.Default.OpenAsync(Range.Location, options);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to open maps", "Okay");
            }
        }
    }
}
