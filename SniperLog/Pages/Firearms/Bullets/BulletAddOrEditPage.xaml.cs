using SniperLog.ViewModels.Firearms.Bullets;
using System.Globalization;

namespace SniperLog.Pages.Firearms.Bullets
{
    public partial class BulletAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public BulletAddOrEditPage(BulletAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(CaliberEntry, static n => n != null);
            _validatorService.TryAddValidation(ManufacturerEntry, static n => n != null);

            _validatorService.TryAddValidation(WeightEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(BulletDiameterEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(BulletLengthEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(BC1Entry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(BC7Entry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));

            await (BindingContext as BulletAddOrEditPageViewModel).RefeshPickersCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}