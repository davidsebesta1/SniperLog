using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Reticles
{
    public partial class ReticlesPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<SightReticle> _sightReticles;

        private readonly DataCacherService<SightReticle> _reticleCacher;
        private readonly DataCacherService<FirearmSight> _sightsCacher;

        public ReticlesPageViewModel(DataCacherService<SightReticle> sightCacher, DataCacherService<FirearmSight> sights)
        {
            PageTitle = "Reticles";

            _reticleCacher = sightCacher;
            _sightsCacher = sights;
        }

        [RelayCommand]
        private async Task RefreshReticles()
        {
            SightReticles = await _reticleCacher.GetAll();
        }

        [RelayCommand]
        private async Task SearchReticles(string text)
        {
            SightReticles = await _reticleCacher.GetAllBy(n => string.IsNullOrEmpty(text) || n.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase));
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
            ObservableCollection<FirearmSight> referencedSights = await _sightsCacher.GetAllBy(n => n.SightReticle_ID == reticle.ID);
            int referencesAmount = referencedSights.Count;

            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {reticle.Name}?{(referencesAmount > 0 ? $"This reticle is referenced on {referencesAmount} sights. " : string.Empty)}This action cannot be undone", "Yes", "No");

            if (res)
            {
                foreach (FirearmSight sight in referencedSights)
                {
                    sight.SightReticle_ID = -1;
                }

                await reticle.DeleteAsync();
                SightReticles.Remove(reticle);
            }
        }
    }
}
