using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.Firearms.MuzzleVelocities;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Firearms.MuzzleVelocities
{
    [QueryProperty(nameof(Firearm), "Firearm")]
    public partial class MuzzleVelocitiesPageViewModel : BaseViewModel
    {
        private DataCacherService<MuzzleVelocity> _muzzleVelocityCacher;

        [ObservableProperty]
        private Firearm _firearm;

        [ObservableProperty]
        private ObservableCollection<MuzzleVelocity> _filtered;

        public MuzzleVelocitiesPageViewModel(DataCacherService<MuzzleVelocity> muzzleVelocityCacher)
        {
            _muzzleVelocityCacher = muzzleVelocityCacher;

            PageTitle = "Muzzle Velocities";
        }

        [RelayCommand]
        private async Task Search()
        {
            if (Firearm != null)
                Filtered = await _muzzleVelocityCacher.GetAllBy(n => n.ReferencedFirearm == Firearm);
            else
                Filtered = await _muzzleVelocityCacher.GetAll();
        }

        [RelayCommand]
        protected async Task ReturnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        protected async Task CreateNew()
        {
            await Shell.Current.GoToAsync(nameof(MuzzleVelocityAddOrEditPage), new Dictionary<string, object>(1) { { "Firearm", Firearm }, { "MuzzleVelocity", null } });
        }

        [RelayCommand]
        protected async Task Edit(MuzzleVelocity mv)
        {
            await Shell.Current.GoToAsync(nameof(MuzzleVelocityAddOrEditPage), new Dictionary<string, object>(1) { { "Firearm", Firearm }, { "MuzzleVelocity", mv } });
        }

        [RelayCommand]
        protected async Task Delete(MuzzleVelocity mv)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {mv.ReferencedFirearm.Name}'s configuration? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await mv.DeleteAsync();
                Filtered.Remove(mv);
            }
        }
    }
}
