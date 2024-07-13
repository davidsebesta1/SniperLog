using SniperLog.ViewModels.SRanges;

namespace SniperLog.Pages.ShootingRanges
{
    public partial class SRangesPage : ContentPage
    {
        public SRangesPage(SRangesPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as SRangesPageViewModel).RefreshShootingRangesCommandCommand.ExecuteAsync(null);
        }
    }
}