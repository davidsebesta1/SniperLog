using CommunityToolkit.Mvvm.Input;
using SniperLog.Config;
using SniperLog.Services.ConnectionToServer;
using System.Net;
using System.Text.RegularExpressions;

namespace SniperLog.ViewModels.Other;

/// <summary>
/// Viewmodel to handle settings page.
/// </summary>
public partial class SettingsPageViewModel : BaseViewModel
{
    /// <summary>
    /// Weather server IPv4 / DNS name.
    /// </summary>
    [ObservableProperty]
    private string _weatherServerTarget;

    /// <summary>
    /// Weather server's port.
    /// </summary>
    [ObservableProperty]
    private ushort _weatherServerPort;

    private readonly ValidatorService _validatorService;

    /// <summary>
    /// Ctor.
    /// </summary>
    public SettingsPageViewModel(ValidatorService validatorService) : base()
    {
        PageTitle = "Settings";
        _validatorService = validatorService;
    }

    /// <summary>
    /// Loads the current settings.
    /// </summary>
    [RelayCommand]
    private void LoadCurrentSettings()
    {
        AppConfig conf = ApplicationConfigService.GetConfig<AppConfig>();
        WeatherServerTarget = conf.ServerHostname;
        WeatherServerPort = conf.ServerPort;
    }

    /// <summary>
    /// Saves the settings.
    /// </summary>
    [RelayCommand]
    private async Task SaveSettings()
    {
        _validatorService.RevalidateAll();
        if (!_validatorService.AllValid)
        {
            await Shell.Current.DisplayAlert("Error", "Please fix any invalid fields and try again.", "Okay");
            return;
        }

        if (IPAddress.TryParse(WeatherServerTarget, out IPAddress address))
        {
            SaveConfigAndService(address, WeatherServerPort);
            await Shell.Current.DisplayAlert("Success", "Settings saved", "Okay");
            return;
        }

        await Shell.Current.DisplayAlert("Error", "Unable to parse IP address", "Okay");
    }

    private void SaveConfigAndService(IPAddress newAddress, ushort port)
    {
        AppConfig config = ApplicationConfigService.GetConfig<AppConfig>();
        config.ServerHostname = newAddress.ToString();
        config.ServerPort = WeatherServerPort;
        ApplicationConfigService.SaveConfig(config);

        ConnectionToDataServer connectionToDataServer = ServicesHelper.GetService<ConnectionToDataServer>();
        connectionToDataServer.IpAddress = newAddress;
        connectionToDataServer.Port = port;
    }
}
