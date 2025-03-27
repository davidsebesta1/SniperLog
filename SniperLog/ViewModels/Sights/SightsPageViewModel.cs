using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.Sights;
using SniperLog.Pages.Sights.ClickSettings;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Sights
{
    public partial class SightsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<FirearmSight> _firearmSights;

        private readonly DataCacherService<FirearmSight> _sightCacher;

        public SightsPageViewModel(DataCacherService<FirearmSight> sightCacher)
        {
            PageTitle = "Sights";

            _sightCacher = sightCacher;
        }

        [RelayCommand]
        private async Task RefreshSights()
        {
            FirearmSights = await _sightCacher.GetAll();
        }

        [RelayCommand]
        private async Task SearchSights(string text)
        {
            FirearmSights = await _sightCacher.GetAllBy(n => string.IsNullOrEmpty(text) || n.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase));
        }

        [RelayCommand]
        private async Task CreateNewSight()
        {
            await Shell.Current.GoToAsync(nameof(SightAddOrEditPage), new Dictionary<string, object>(1) { { "Sight", null } });
        }

        [RelayCommand]
        private async Task EditSight(FirearmSight sight)
        {
            await Shell.Current.GoToAsync(nameof(SightAddOrEditPage), new Dictionary<string, object>(1) { { "Sight", sight } });
        }

        [RelayCommand]
        private async Task DeleteSight(FirearmSight sight)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {sight.Name}? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await sight.DeleteAsync();
                FirearmSights.Remove(sight);
            }
        }

        [RelayCommand]
        private async Task GoToZeroSettings(FirearmSight sight)
        {
            await Shell.Current.GoToAsync(nameof(SightClickSettingsPage), new Dictionary<string, object>(1) { { "Sight", sight } });
        }
    }
}
