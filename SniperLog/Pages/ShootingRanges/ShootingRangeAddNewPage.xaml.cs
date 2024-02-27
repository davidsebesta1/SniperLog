using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mopups.Pages;
using Mopups.Services;
using SniperLog.Extensions;
using SniperLog.Models;
using SniperLog.Services;
using System.Security.Cryptography;

namespace SniperLog.Pages;

public partial class ShootingRangeAddNewPage : PopupPage
{
    private DataService<ShootingRange> _dataService;
    public ShootingRangeAddNewPage(DataService<ShootingRange> service)
    {
        InitializeComponent();
        _dataService = service;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {

            string name = ShootingRangeName.Text;
            string address = ShootingRangeAddress.Text;
            double latitude = double.Parse(ShootingRangeLang.Text);
            double longitude = double.Parse(ShootingRangeLong.Text);
            string imagePathRel = string.Empty;

            string fullPath = ImageFilePathLabel.Text;
            if (!string.IsNullOrEmpty(fullPath))
            {
                imagePathRel = Path.Combine("Data", "ShootingRanges", name, $"BackgroundImage{Path.GetExtension(fullPath)}");
            }

            ShootingRange range = new ShootingRange(name, address, latitude, longitude, imagePathRel);
            if (!string.IsNullOrEmpty(imagePathRel)) await range.SaveBackgroundImageAsync(fullPath);
            await _dataService.SaveOrUpdateAsync(range);

            await MopupService.Instance.PopAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
        }
    }

    private async void BackgroundImagePickerButton_Clicked(object sender, EventArgs e)
    {
        FileResult? result = await FilePicker.Default.PickAsync(new PickOptions()
        {
            PickerTitle = "Background image",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
        {
            return;
        }

        ImageFilePathLabel.Text = result.FullPath;

        BackgroundImageTest.Source = ImageSource.FromFile(result.FullPath);
    }
}