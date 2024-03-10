using CommunityToolkit.Mvvm.ComponentModel;
using SniperLog.Models;
using SniperLog.Services;
using System;
using System.Collections.Generic;
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

        public SubRangesSettingsViewModel(DataCacherService<ShootingRange> shootingRangeService, DataCacherService<SubRange> subRangeService) : base()
        {
            _shootingRangesService = shootingRangeService;
            _subRangesService = subRangeService;

        }
    }
}