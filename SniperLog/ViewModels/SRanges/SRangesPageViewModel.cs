using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.SRanges
{
    public partial class SRangesPageViewModel : BaseViewModel
    {
        private readonly DataCacherService<ShootingRange> _sRangeDataCacher;

        [ObservableProperty]
        private ObservableCollection<ShootingRange> _shootingRanges;

        public SRangesPageViewModel(DataCacherService<ShootingRange> sRangeDataCacher) : base()
        {
            _sRangeDataCacher = sRangeDataCacher;
        }

        [RelayCommand]
        private async Task RefreshShootingRangesCommand()
        {
            ShootingRanges = null;
            ShootingRanges = await _sRangeDataCacher.GetAll();
        }

        [RelayCommand]
        private async Task ShootingRangesSearchCommand(string input)
        {
            ShootingRanges = null;
            ShootingRanges = await _sRangeDataCacher.GetAllBy(n => n.Name.Contains(input, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}