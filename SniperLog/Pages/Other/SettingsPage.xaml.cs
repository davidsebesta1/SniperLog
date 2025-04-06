using SniperLog.ViewModels.Other;

namespace SniperLog.Pages.Other;

public partial class SettingsPage : ContentPage
{
    private readonly ValidatorService _validatorService;

    public SettingsPage(SettingsPageViewModel vm, ValidatorService validatorService)
    {
        InitializeComponent();
        BindingContext = vm;
        _validatorService = validatorService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        (BindingContext as SettingsPageViewModel).LoadCurrentSettingsCommand.Execute(null);

        _validatorService.TryAddValidation(WeatherServerIP, (static n => !string.IsNullOrEmpty((string)n)));
        _validatorService.TryAddValidation(WeatherServerPort, (static n => ushort.TryParse((string)n, out ushort port)));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _validatorService.ClearAll();
    }
}