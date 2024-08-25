using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Sights.ClickSettings
{
    [QueryProperty(nameof(Sight), "Sight")]
    public partial class SightClickSettingsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        protected ObservableCollection<FirearmSightSetting> _sightClickSettings;

        [ObservableProperty]
        private FirearmSight _sight;

        protected readonly DataCacherService<FirearmSightSetting> _clickCacher;

        public SightClickSettingsPageViewModel(DataCacherService<FirearmSightSetting> clickCacher)
        {
            _clickCacher = clickCacher;
        }

        partial void OnSightChanged(FirearmSight value)
        {
            PageTitle = Sight?.Name ?? string.Empty;

            RefreshClickSettings();
        }

        [RelayCommand]
        protected async Task RefreshClickSettings()
        {
            SightClickSettings = new ObservableCollection<FirearmSightSetting>((await _clickCacher.GetAllBy(n => n.FirearmSight_ID == Sight.ID)).OrderBy(n => n.Distance));

            for (int i = 0; i < SightClickSettings.Count; i++)
            {
                if (i == 0)
                {
                    SightClickSettings[0].PreviousSetting = null;
                    continue;
                }

                SightClickSettings[i].PreviousSetting = SightClickSettings[i - 1];
            }
        }

        [RelayCommand]
        protected async Task ReturnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        protected async Task EditClickSettings(FirearmSightSetting clickSettings)
        {
            await Shell.Current.GoToAsync("SightClickSettings/AddOrEdit", new Dictionary<string, object>(2) { { "Setting", clickSettings }, { "Sight", Sight } });
        }

        [RelayCommand]
        protected async Task DeleteClickSettings(FirearmSightSetting clickSettings)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete this setting? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await clickSettings.DeleteAsync();
                SightClickSettings.Remove(clickSettings);
            }
        }

        [RelayCommand]
        protected async Task CreateNewClickSettings()
        {
            await Shell.Current.GoToAsync("SightClickSettings/AddOrEdit", new Dictionary<string, object>(21) { { "Setting", null }, { "Sight", Sight } });
        }
    }
}
