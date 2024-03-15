using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLog.Models;
using SniperLog.Pages.SubRanges;
using SniperLog.Services;
using System.Collections.ObjectModel;
namespace SniperLog.ViewModels
{
    [QueryProperty("SelectedRange", "BaseShootingRange")]
    public partial class SubRangesSettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRange _selectedRange;

        private DataCacherService<ShootingRange> _shootingRangesService;
        private DataCacherService<SubRange> _subRangesService;

        [ObservableProperty]
        private ObservableCollection<SubRange> _subRanges;

        public SubRangesSettingsViewModel(DataCacherService<ShootingRange> shootingRangeService, DataCacherService<SubRange> subRangeService) : base()
        {
            _shootingRangesService = shootingRangeService;
            _subRangesService = subRangeService;
        }

        [RelayCommand]
        private async Task RefreshSubRangesCommand()
        {
            SubRanges = _subRangesService.GetAllBy(n => n.ShootingRange_ID == SelectedRange.ID);
        }

        [RelayCommand]
        private async Task EditSubRange(SubRange sub)
        {
            if (IsBusy) return;

            try
            {
                await MopupService.Instance.PushAsync(new SubRangeEditPage(sub));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}