using CommunityToolkit.Mvvm.Input;
using SniperLog.Pages.Firearms.Bullets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Firearms.Bullets
{
    public partial class BulletsPageViewModel : BaseViewModel
    {
        private readonly DataCacherService<Bullet> _bulletsCacher;

        [ObservableProperty]
        private ObservableCollection<Bullet> _bullets;

        public BulletsPageViewModel(DataCacherService<Bullet> bulletsCacher)
        {
            _bulletsCacher = bulletsCacher;
            PageTitle = "Bullets";
        }

        [RelayCommand]
        protected async Task Refresh()
        {
            Bullets = await _bulletsCacher.GetAll();
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
             await Shell.Current.GoToAsync(nameof(BulletAddOrEditPage), new Dictionary<string, object>(1) { { "Bullet", null } });
        }

        [RelayCommand]
        protected async Task Edit(Bullet ammo)
        {
              await Shell.Current.GoToAsync(nameof(BulletAddOrEditPage), new Dictionary<string, object>(1) { { "Bullet", ammo } });
        }

        [RelayCommand]
        protected async Task Delete(Bullet ammo)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete {ammo.ReferencedFirearmCaliber.Caliber}? This action cannot be undone", "Yes", "No");

            if (res)
            {
                await ammo.DeleteAsync();
                Bullets.Remove(ammo);
            }
        }
    }
}
