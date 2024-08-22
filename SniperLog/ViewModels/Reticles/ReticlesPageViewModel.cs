using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Reticles
{
    public partial class ReticlesPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<SightReticle> _sightReticles;

        private readonly DataCacherService<SightReticle> _sightCacher;

        public ReticlesPageViewModel(DataCacherService<SightReticle> sightCacher)
        {
            PageTitle = "Reticles";

            _sightCacher = sightCacher;
        }

        [RelayCommand]
        private async Task RefreshReticles()
        {
            SightReticles = await _sightCacher.GetAll();
        }

        [RelayCommand]
        private async Task SearchReticles(string text)
        {
            SightReticles = await _sightCacher.GetAllBy(n => string.IsNullOrEmpty(text) || n.Name.Contains(text));
        }

        [RelayCommand]
        private async Task CreateNewReticle()
        {
            await Shell.Current.GoToAsync("Reticles/AddOrEdit", new Dictionary<string, object>(1) { { "Reticle", null } });
        }

        [RelayCommand]
        private async Task EditReticle(SightReticle reticle)
        {
            await Shell.Current.GoToAsync("Reticles/AddOrEdit", new Dictionary<string, object>(1) { { "Reticle", reticle } });
        }

        [RelayCommand]
        private async Task DeleteReticle(SightReticle reticle)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {reticle.Name}? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await reticle.DeleteAsync();
                SightReticles.Remove(reticle);
            }
        }
    }
}
