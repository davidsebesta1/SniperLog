using CommunityToolkit.Maui.Markup;
using SniperLog.Extensions;
using SniperLog.ViewModels;
using Microsoft.Maui.Controls.Maps;

namespace SniperLog.Pages.ShootingRanges;

public partial class ShootingRangeDetailsPage : ContentPage
{
    public ShootingRangeDetailsPage(ShootingRangeDetailsViewModel viewmodel)
    {
        BindingContext = viewmodel;
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            ShootingRangeDetailsViewModel viewModel = (BindingContext as ShootingRangeDetailsViewModel);
            ShootingRangeImage.Source = ImageSource.FromFile(AppDataFileLoader.GetPathFromAppData(viewModel.SelectedRange.RelativeImagePathFromAppdata));
            Map.Pins.Add(new Pin()
            {
                Address = viewModel.SelectedRange.Address,
                Label = $"Shooting Range {viewModel.SelectedRange.Name}",
                Location = viewModel.SelectedRange.Location

            });
        }
        catch (ArgumentException ex)
        {
            //TestLabel.Text = ex.Message;

            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");

        }
    }
}