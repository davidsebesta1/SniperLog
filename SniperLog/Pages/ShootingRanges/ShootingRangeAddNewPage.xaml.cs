using Mopups.Pages;
using Mopups.Services;
using SniperLog.Models;

namespace SniperLog.Pages;

public partial class ShootingRangeAddNewPage : PopupPage
{
    public ShootingRangeAddNewPage()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string name = ShootingRangeName.Text;
        string address = ShootingRangeAddress.Text;
        double latitude = double.Parse(ShootingRangeLang.Text);
        double longitude = double.Parse(ShootingRangeLong.Text);


        ShootingRange range = new ShootingRange(name, address, latitude, longitude, "");
        await range.SaveAsync();

        await MopupService.Instance.PopAsync();
    }
}