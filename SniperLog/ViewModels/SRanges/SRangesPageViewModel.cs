using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.ShootingRanges;
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
                try
                {
                    await range.TrySendWeatherRequestMessage();
                }
                catch (TimeoutException e)
                {
                    continue;
                }
                catch (Exception e)
                {
                    await Shell.Current.DisplayAlert("Error", e.Message, "Okay");
                }
            }
        }

        [RelayCommand]
        private async Task ShootingRangesSearch(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                ShootingRanges = await _sRangeDataCacher.GetAll();
                return;
            }
            ShootingRanges = await _sRangeDataCacher.GetAllBy(n => n.Name.Contains(input, StringComparison.CurrentCultureIgnoreCase));
        }

        [RelayCommand]
        private async Task GoToDetails(ShootingRange range)
        {
            await Shell.Current.GoToAsync(nameof(SRangeDetailsPage), new Dictionary<string, object>(1) { { "ShootingRange", range } });
        }

        [RelayCommand]
        private async Task AddNewRange()
        {
            await Shell.Current.GoToAsync(nameof(SRangesAddOrEditPage), new Dictionary<string, object>(1) { { "ShootingRange", null } });
        }

        [RelayCommand]
        private async Task EditRange(ShootingRange range)
        {
            await Shell.Current.GoToAsync(nameof(SRangesAddOrEditPage), new Dictionary<string, object>(1) { { "ShootingRange", range } });
        }

        [RelayCommand]
        private async Task DeleteRange(ShootingRange range)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete shooting range {range.Name}?", "Yes", "No");

            if (res)
            {
                await range.DeleteAsync();
                ShootingRanges.Remove(range);
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