using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.Firearms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Firearms
{
    public partial class AmmunitionsPageViewModel : BaseViewModel
    {
        private readonly DataCacherService<Ammunition> _ammunitionCacherService;

        [ObservableProperty]
        private ObservableCollection<Ammunition> _ammunitions;

        public AmmunitionsPageViewModel(DataCacherService<Ammunition> ammunitionCacherService)
        {
            _ammunitionCacherService = ammunitionCacherService;

            PageTitle = "Ammunitions";
        }

        [RelayCommand]
        protected async Task Refresh()
        {
            Ammunitions = await _ammunitionCacherService.GetAll();
        }

        [RelayCommand]
        protected async Task Search(string text)
        {

        }

        [RelayCommand]
        protected async Task ReturnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        protected async Task CreateNew()
        {
            await Shell.Current.GoToAsync(nameof(AmmunitionAddOrEditPage), new Dictionary<string, object>(1) { { "Ammunition", null } });
        }

        [RelayCommand]
        protected async Task Edit(Ammunition ammo)
        {
            await Shell.Current.GoToAsync(nameof(AmmunitionAddOrEditPage), new Dictionary<string, object>(1) { { "Ammunition", ammo } });
        }

        [RelayCommand]
        protected async Task Delete(Ammunition ammo)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {ammo.ReferencedBullet.ReferencedFirearmCaliber.Caliber}? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await ammo.DeleteAsync();
                Ammunitions.Remove(ammo);
            }
        }
    }
}
