using SniperLog.ViewModels;

namespace SniperLog.Pages.ShootingRanges;

public partial class ShootingRangeSubRangesPage : ContentPage
{
    public ShootingRangeSubRangesPage(SubRangesSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as SubRangesSettingsViewModel).RefreshSubRangesCommandCommand.Execute(null);
    }
}