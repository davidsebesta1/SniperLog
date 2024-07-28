using SniperLog.ViewModels.SRanges;
using System.Collections.Specialized;

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

            SRangesPageViewModel? sRangesPageViewModel = BindingContext as SRangesPageViewModel;
            await sRangesPageViewModel.RefreshShootingRangesCommand.ExecuteAsync(null);
        }

        private async void ShootingRanges_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                return;
            }

            foreach (object item in e.NewItems)
            {
                ShootingRange range = (ShootingRange)item;
                await range.TrySendWeatherRequestMessage();
            }
        }
    }
}