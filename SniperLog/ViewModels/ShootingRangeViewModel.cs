using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLog.Models;
using SniperLog.Pages;
using SniperLog.Pages.ShootingRanges;
using SniperLog.Services;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels
{
    public partial class ShootingRangeViewModel : BaseViewModel
    {
        public ObservableCollection<ShootingRange> ShootingRanges => _dataFetcher.Data;

        private readonly DataService<ShootingRange> _dataFetcher;

        [ObservableProperty]
        private bool _isRefreshing;

        public ShootingRangeViewModel(DataService<ShootingRange> dataFetcher) : base()
        {
            this.PageTitle = "Shooting Ranges";
            this._dataFetcher = dataFetcher;
        }

        [RelayCommand]
        private async Task GetShootingRanges()
        {
            if (IsBusy) return;

            try
            {
                await _dataFetcher.GetAllToCollection();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task AddNewRange()
        {
            if (IsBusy) return;

            try
            {
                await MopupService.Instance.PushAsync(new ShootingRangeAddNewPage(_dataFetcher));
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(ShootingRange srange)
        {
            if (srange == null) return;

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"TappedShootingRange", srange }
            };

            await Shell.Current.GoToAsync($"{nameof(ShootingRangeDetailsPage)}?id={srange.ID}", true, parameters);
        }
    }
}