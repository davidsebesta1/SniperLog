using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using SniperLog.Models;
using SniperLog.Services;
using SniperLog.Services.Database;

namespace SniperLog.ViewModels
{
    [QueryProperty("SelectedRange", "TappedShootingRange")]
    public partial class ShootingRangeDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRange _selectedRange;

        private DataService<ShootingRange> _dataService;

        public ShootingRangeDetailsViewModel(DataService<ShootingRange> service) : base()
        {
            _dataService = service;
        }

        [RelayCommand]
        private async Task DeleteThisRange()
        {
            bool confirm = await Shell.Current.DisplayAlert("Delete Confirmation", $"Are you sure you want to delete shooting range {_selectedRange.Name}", "Delete", "Cancel");

            if (confirm)
            {
                await _dataService.DeleteAsync(_selectedRange);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
