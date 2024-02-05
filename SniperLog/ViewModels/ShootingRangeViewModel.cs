using CommunityToolkit.Mvvm.Input;
using SniperLog.Models;
using SniperLog.Services;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels
{
    public partial class ShootingRangeViewModel : BaseViewModel
    {
        public ObservableCollection<ShootingRange> ShootingRanges = new ObservableCollection<ShootingRange>();

        private DataFetcherService<ShootingRange> _dataFetcher;

        public ShootingRangeViewModel(DataFetcherService<ShootingRange> dataFetcher)
        {
            this.PageTitle = "Shooting Ranges";
            this._dataFetcher = dataFetcher;
        }

        [RelayCommand]
        public async Task GetShootingRanges()
        {
            if (IsBusy) return;

            try
            {
                ShootingRanges = await _dataFetcher.GetAll();
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
    }
}