using Mopups.Pages;
using Mopups.Services;
using SniperLog.Models;
using SniperLog.Services;

namespace SniperLog.Pages;

public partial class ShootingRangeAddNewPage : PopupPage
{
    private DataCacherService<ShootingRange> _dataService;

    private Dictionary<string, bool> _valuesValidation = new Dictionary<string, bool>()
    {
        {"Name", false },
        {"Address", true },
        {"Latitude", false },
        {"Longitude", false },
        {"RelativeImagePathFromAppdata", false }
    };
    public ShootingRangeAddNewPage(DataCacherService<ShootingRange> service)
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
            _dataService.AddOrUpdateAsync(range);

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

    private void ShootingRangeName_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool res = !string.IsNullOrEmpty(e.NewTextValue);
        _valuesValidation["Name"] = res;
        NameErrText.IsVisible = res;
    }

    private void ShootingRangeAddress_TextChanged(object sender, TextChangedEventArgs e)
    {
        _valuesValidation["Address"] = true;
    }

    private void ShootingRangeLang_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!double.TryParse(e.NewTextValue, out double result))
        {
            _valuesValidation["Latitude"] = true;
            NameErrText.IsVisible = false;
            return;
        }

        NameErrText.IsVisible = true;
    }//do the longitude etc
}