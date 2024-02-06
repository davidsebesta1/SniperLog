using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLog.Models;
using SniperLog.Pages;
using SniperLog.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SniperLog.ViewModels
{
    public partial class ShootingRangeViewModel : BaseViewModel
    {
        public ObservableCollection<ShootingRange> ShootingRanges { get; } = new ObservableCollection<ShootingRange>();

        private DataFetcherService<ShootingRange> _dataFetcher;

        public ShootingRangeViewModel(DataFetcherService<ShootingRange> dataFetcher)
        {
            this.PageTitle = "Shooting Ranges";
            this._dataFetcher = dataFetcher;
        }

        [RelayCommand]
        private async Task GetShootingRanges()
        {
            if (IsBusy) return;

            Trace.WriteLine("Test");
            try
            {
                await _dataFetcher.GetAll(ShootingRanges);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddNewRange()
        {
            if (IsBusy) return;

            try
            {
                await MopupService.Instance.PushAsync(new ShootingRangeAddNewPage());
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}