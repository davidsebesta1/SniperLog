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

            _validatorService.TryAddValidation(NameEntry, static n => !string.IsNullOrEmpty((string)n));
            _validatorService.TryAddValidation(FirearmTypeEntry, static n => n != null);
            _validatorService.TryAddValidation(ManufacturerEntry, static n => n != null);
            _validatorService.TryAddValidation(CaliberEntry, static n => n != null);
            _validatorService.TryAddValidation(SightEntry, static n => n != null);
            _validatorService.TryAddValidation(SightOffsetEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));

            _validatorService.TryAddValidation(TotalLenEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(BarrelLenEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(WeightEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));

            await (BindingContext as FirearmAddOrEditPageViewModel).RefeshPickersCommand.ExecuteAsync(null);
            //(BindingContext as FirearmAddOrEditPageViewModel).Firearm = new Firearm(1, 1, 1, 1, "a", "", "", 1, 1, "", 1, false, 1);
            //(BindingContext as FirearmAddOrEditPageViewModel).Firearm = null;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}