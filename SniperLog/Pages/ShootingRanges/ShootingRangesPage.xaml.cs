using SniperLog.ViewModels;

namespace SniperLog.Pages;

public partial class ShootingRangesPage : ContentPage
{
    public ShootingRangesPage(ShootingRangeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ShootingRangeViewModel viewModel = BindingContext as ShootingRangeViewModel;
        viewModel.GetShootingRangesCommand.Execute(null);
    }
}