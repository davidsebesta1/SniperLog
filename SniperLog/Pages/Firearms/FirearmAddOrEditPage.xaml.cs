using SniperLog.ViewModels.Firearms;
using System.Globalization;

namespace SniperLog.Pages.Firearms
{
    public partial class FirearmAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public FirearmAddOrEditPage(FirearmAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(NameEntry, n => !string.IsNullOrEmpty((string)n));
            _validatorService.TryAddValidation(FirearmTypeEntry, n => n != null);
            _validatorService.TryAddValidation(ManufacturerEntry, n => n != null);
            _validatorService.TryAddValidation(CaliberEntry, n => n != null);

            _validatorService.TryAddValidation(TotalLenEntry, n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(BarrelLenEntry, n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(WeightEntry, n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));

            await (BindingContext as FirearmAddOrEditPageViewModel).RefeshPickersCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}