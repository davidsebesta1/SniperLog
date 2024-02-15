using CommunityToolkit.Mvvm.ComponentModel;
using SniperLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels
{
    [QueryProperty("TappedShootingRange", "SelectedRange")]
    public partial class ShootingRangeDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRange _selectedRange;
    }
}
