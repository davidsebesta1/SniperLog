using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SniperLog.Models;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Services;

namespace SniperLog.ViewModels
{
    [QueryProperty("SelectedRange", "TappedShootingRange")]
    public partial class ShootingRangeDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRange _selectedRange;

        private DataCacherService<ShootingRange> _dataService;

        public ShootingRangeDetailsViewModel(DataCacherService<ShootingRange> service) : base()
        {
            _dataService = service;
        }

        [RelayCommand]
        private async Task DeleteThisRange()
        {
            bool confirm = await Shell.Current.DisplayAlert("Delete Confirmation", $"Are you sure you want to delete shooting range {SelectedRange.Name}", "Delete", "Cancel");

            if (confirm)
            {
                await _dataService.RemoveAsync(SelectedRange);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task GoToSubRangeSettings()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"BaseShootingRange", SelectedRange }
            };

            await Shell.Current.GoToAsync(nameof(ShootingRangeSubRangesPage), true, parameters);
        }
    }
}
