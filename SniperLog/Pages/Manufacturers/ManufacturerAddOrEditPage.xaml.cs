using SniperLog.ViewModels.Manufacturers;

namespace SniperLog.Pages.Manufacturers
{
    public partial class ManufacturerAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public ManufacturerAddOrEditPage(ManufacturerAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(NameEntry, static n => !string.IsNullOrEmpty((string)n));
            _validatorService.TryAddValidation(CountryEntry, static n => n != null);

            await (BindingContext as ManufacturerAddOrEditPageViewModel).RefreshCountriesCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}