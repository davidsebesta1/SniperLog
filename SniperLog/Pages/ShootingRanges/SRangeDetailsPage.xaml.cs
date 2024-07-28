using SniperLog.ViewModels.SRanges;

namespace SniperLog.Pages.ShootingRanges
{
    public partial class SRangeDetailsPage : ContentPage
    {
        public SRangeDetailsPage(SRangeDetailsPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}