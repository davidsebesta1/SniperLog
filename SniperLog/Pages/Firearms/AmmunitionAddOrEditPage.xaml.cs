using SniperLog.ViewModels.Firearms;
using System.Globalization;

namespace SniperLog.Pages.Firearms
{
    public partial class AmmunitionAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public AmmunitionAddOrEditPage(AmmunitionAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(BulletEntry, static n => n != null);

            _validatorService.TryAddValidation(TotalLenEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));
            _validatorService.TryAddValidation(PowerWeightEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d));

            await (BindingContext as AmmunitionAddOrEditPageViewModel).RefeshPickersCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}