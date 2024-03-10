using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SniperLog.Models;
using SniperLog.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            SubRanges = _subRangesService.GetAllBy(n => n.ShootingRange_ID == _selectedRange.ID);
        }
    }
}