using Microsoft.Maui.Controls.Maps;
using SniperLog.Models;
using SniperLog.ViewModels;

namespace SniperLog.Pages.ShootingRanges;

public partial class ShootingRangeDetailsPage : ContentPage
{
    public ShootingRange SelectedRange => (BindingContext as ShootingRangeDetailsViewModel).SelectedRange;
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
            //NameLabel.Text = string.Format("{0} {1}", "Shooting Range", viewModel.SelectedRange.Name);

            //ShootingRangeImage.Source = ImageSource.FromFile(AppDataFileHelper.GetPathFromAppData(viewModel.SelectedRange.RelativeImagePathFromAppdata));
            LocationMap.Pins.Add(new Pin()
            {
                Location = SelectedRange.Location,
                Label = SelectedRange.Name,
                Address = SelectedRange.Address
            });
        }
        catch (ArgumentException ex)
        {
            //TestLabel.Text = ex.Message;

            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
        }
    }
}