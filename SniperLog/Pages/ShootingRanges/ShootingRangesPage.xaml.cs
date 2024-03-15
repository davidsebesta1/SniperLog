using SniperLog.ViewModels;

namespace SniperLog.Pages.ShootingRanges;

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

        try
        {
            ShootingRangeViewModel viewModel = BindingContext as ShootingRangeViewModel;
            viewModel.GetShootingRangesCommand.Execute(null);

        }
        catch (Exception ex)
        {

        }
    }
}