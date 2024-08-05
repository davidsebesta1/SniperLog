using SniperLog.ViewModels.SRanges.Subranges;

namespace SniperLog.Pages.ShootingRanges.Subranges
{
    public partial class SubRangesPage : ContentPage
    {
        public SubRangesPage(SubRangesPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as SubRangesPageViewModel).RefreshSubRangesCommand.ExecuteAsync(null);
        }
    }
}