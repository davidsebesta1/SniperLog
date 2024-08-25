using SniperLog.ViewModels.Sights;
using System.Globalization;

namespace SniperLog.Pages.Sights
{
    public partial class SightAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public SightAddOrEditPage(SightAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(NameEntry, n => !string.IsNullOrEmpty((string)n));
            _validatorService.TryAddValidation(ReticleEntry, n => n != null);
            _validatorService.TryAddValidation(ClickTypeEntry, n => true);
            _validatorService.TryAddValidation(OneClickValEntry, n => !string.IsNullOrEmpty((string)n) && double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res > 0d);
            _validatorService.TryAddValidation(ManuEntry, n => n != null);

            await (BindingContext as SightAddOrEditPageViewModel).RefreshManufacturersCommand.ExecuteAsync(null);
            await (BindingContext as SightAddOrEditPageViewModel).RefreshClickTypesCommand.ExecuteAsync(null);
            await (BindingContext as SightAddOrEditPageViewModel).RefreshSightReticlesCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}