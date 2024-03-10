using CommunityToolkit.Mvvm.ComponentModel;
using SniperLog.Models;
using SniperLog.ViewModels;

namespace SniperLog.Pages.ShootingRanges;

public partial class ShootingRangeSubRangesPage : ContentPage
{
    public ShootingRangeSubRangesPage(SubRangesSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}