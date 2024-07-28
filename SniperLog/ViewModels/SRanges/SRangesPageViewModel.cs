using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.SRanges
{
    public partial class SRangesPageViewModel : BaseViewModel
    {
        private readonly DataCacherService<ShootingRange> _sRangeDataCacher;

        [ObservableProperty]
        private ObservableCollection<ShootingRange> _shootingRanges;

        public SRangesPageViewModel(DataCacherService<ShootingRange> sRangeDataCacher) : base()
        {
            _sRangeDataCacher = sRangeDataCacher;
        }

        [RelayCommand]
        private async Task RefreshShootingRanges()
        {
            ShootingRanges = null;
            ShootingRanges = await _sRangeDataCacher.GetAll();

            foreach (ShootingRange range in ShootingRanges)
            {
                await range.TrySendWeatherRequestMessage();
            }
        }

        [RelayCommand]
        private async Task ShootingRangesSearch(string input)
        {
            ShootingRanges = null;
            ShootingRanges = await _sRangeDataCacher.GetAllBy(n => n.Name.Contains(input, StringComparison.CurrentCultureIgnoreCase));
        }

        [RelayCommand]
        private async Task GoToDetails(ShootingRange range)
        {
            await Shell.Current.GoToAsync("RangeDetails", new Dictionary<string, object>() { { "ShootingRange", range } });
        }

        [RelayCommand]
        private async Task AddNewRange()
        {
            await Shell.Current.GoToAsync("AddOrEditRange", new Dictionary<string, object>() { { "ShootingRange", null } });
        }

        [RelayCommand]
        private async Task EditRange(ShootingRange range)
        {
            await Shell.Current.GoToAsync("AddOrEditRange", new Dictionary<string, object>() { { "ShootingRange", range } });
        }

        [RelayCommand]
        private async Task DeleteRange(ShootingRange range)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete shooting range {range.Name}?", "Yes", "No");

            if (res)
            {
                await range.DeleteAsync();
            }
        }

        [RelayCommand]
        private async Task ToggleFavouriteRange(ShootingRange range)
        {
            range.IsMarkedAsFavourite = !range.IsMarkedAsFavourite;
            await range.SaveAsync();
        }
    }
}