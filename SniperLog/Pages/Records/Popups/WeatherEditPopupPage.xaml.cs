using Mopups.Pages;
using SniperLog.ViewModels.Records;

namespace SniperLog.Pages.Records.Popups
{
    public partial class WeatherEditPopupPage : PopupPage
    {
        private readonly ValidatorService _validatorService;

        public WeatherEditPopupPage(WeatherEditPopupPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(TemperatureEntry, n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, out double d) && d > -273.15d));
            _validatorService.TryAddValidation(PressureEntry, n => string.IsNullOrEmpty((string)n) || (ushort.TryParse((string)n, out ushort d) && d >= 0));
            _validatorService.TryAddValidation(HumidityEntry, n => string.IsNullOrEmpty((string)n) || (byte.TryParse((string)n, out byte d) && d >= 0 && d <= 100));
            _validatorService.TryAddValidation(WindspeedEntry, n => string.IsNullOrEmpty((string)n) || (byte.TryParse((string)n, out byte d) && d >= 0));
            _validatorService.TryAddValidation(WindDirEntry, n => string.IsNullOrEmpty((string)n) || (ushort.TryParse((string)n, out ushort d) && d >= 0));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}