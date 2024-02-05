using SniperLog.ViewModels;

namespace SniperLog.Pages;

public partial class ShootingRangesPage : ContentPage
{
    public ShootingRangesPage(ShootingRangeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}