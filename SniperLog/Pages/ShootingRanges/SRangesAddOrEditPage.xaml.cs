namespace SniperLog.Pages.ShootingRanges
{
    public partial class SRangesAddOrEditPage : ContentPage
    {
        public SRangesAddOrEditPage(SRangesAddOrEditPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}