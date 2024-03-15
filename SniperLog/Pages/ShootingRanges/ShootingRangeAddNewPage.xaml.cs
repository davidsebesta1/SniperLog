using Mopups.Pages;
using Mopups.Services;
using SniperLog.Extensions;
using SniperLog.Extensions.Maui;
using SniperLog.Models;
using SniperLog.Services;

namespace SniperLog.Pages.ShootingRanges;

public partial class ShootingRangeAddNewPage : PopupPage
{
    private readonly DataCacherService<ShootingRange> _dataService;

    private readonly Dictionary<string, bool> _valuesValidation = new Dictionary<string, bool>()
    {
        {"Name", false },
        {"Address", true },
        {"Latitude", false },
        {"Longitude", false },
        {"RelativeImagePathFromAppdata", true }
    };
    public ShootingRangeAddNewPage(DataCacherService<ShootingRange> service)
    {
        InitializeComponent();
        _dataService = service;
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_valuesValidation.Values.Any(n => n == false))
            {
                await Shell.Current.DisplayAlert("Error", "Please fill out mandatory fields and fix any invalid inputs", "Okay");
                return;
            }

            string name = ShootingRangeName.Text;
            string address = ShootingRangeAddress.Text;
            double latitude = double.Parse(ShootingRangeLat.Text);
            double longitude = double.Parse(ShootingRangeLong.Text);
            string imagePathRel = string.Empty;


            string fullPath = ImageFilePathLabel.Text;
            if (!string.IsNullOrEmpty(fullPath))
            {
                imagePathRel = Path.Combine("Data", "ShootingRanges", name, $"BackgroundImage{Path.GetExtension(fullPath)}");
            }

            ShootingRange range = new ShootingRange(name, address, latitude, longitude, imagePathRel);
            if (!string.IsNullOrEmpty(imagePathRel)) await range.SaveBackgroundImageAsync(fullPath);
            await _dataService.AddOrUpdateAsync(range);

            await MopupService.Instance.PopAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
        }
    }

    private async void ReturnButton_Clicked(object sender, EventArgs e)
    {
        try
        {
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

        BackgroundImageTest.Source = ImageSource.FromFile(result.FullPath);
        InputValidatorHelper.ValidationCheck(result.FullPath, BackgroundImageErrText, _valuesValidation, "RelativeImagePathFromAppdata", FileExtensionsChecker.IsImageFile);
    }

    private void ShootingRangeName_TextChanged(object sender, TextChangedEventArgs e)
    {
        InputValidatorHelper.ValidationCheck(e.NewTextValue, NameErrText, _valuesValidation, "Name", n => !string.IsNullOrEmpty(n));
    }

    private void ShootingRangeAddress_TextChanged(object sender, TextChangedEventArgs e)
    {
        _valuesValidation["Address"] = true;
    }

    private void ShootingRangeLat_TextChanged(object sender, TextChangedEventArgs e)
    {
        InputValidatorHelper.ValidationCheck(e.NewTextValue, LatitudeErrText, _valuesValidation, "Latitude", n => double.TryParse(n, out double res));
    }

    private void ShootingRangeLong_TextChanged(object sender, TextChangedEventArgs e)
    {
        InputValidatorHelper.ValidationCheck(e.NewTextValue, LongitudeErrText, _valuesValidation, "Longitude", n => double.TryParse(n, out double res));
    }
}