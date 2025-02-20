using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.Firearms;
using SniperLog.Pages.Firearms.MuzzleVelocities;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Firearms
{
    public partial class FirearmsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<Firearm> _firearms;

        private readonly DataCacherService<Firearm> _firearmCacher;

        public FirearmsPageViewModel(DataCacherService<Firearm> firearmCacher)
        {
            PageTitle = "Firearms";

            _firearmCacher = firearmCacher;
        }

        [RelayCommand]
        private async Task Refresh()
        {
            Firearms = await _firearmCacher.GetAll();
        }

        [RelayCommand]
        private async Task Search(string text)
        {
            Firearms = await _firearmCacher.GetAllBy(n => string.IsNullOrEmpty(text) || n.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase));
        }

        [RelayCommand]
        private async Task OpenMuzzleVelocities(Firearm firearm)
        {
            await Shell.Current.GoToAsync(nameof(MuzzleVelocitiesPage), new Dictionary<string, object>(1) { { "Firearm", firearm } });
        }

        [RelayCommand]
        private async Task CreateNew()
        {
            await Shell.Current.GoToAsync(nameof(FirearmAddOrEditPage), new Dictionary<string, object>(1) { { "Firearm", null } });
        }

        [RelayCommand]
        private async Task Edit(Firearm firearm)
        {
            await Shell.Current.GoToAsync(nameof(FirearmAddOrEditPage), new Dictionary<string, object>(1) { { "Firearm", firearm } });
        }

        [RelayCommand]
        private async Task Delete(Firearm firearm)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {firearm.Name}? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await firearm.DeleteAsync();
            }
        }
    }
}
