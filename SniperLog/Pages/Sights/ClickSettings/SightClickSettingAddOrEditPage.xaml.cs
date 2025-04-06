using SniperLog.ViewModels.Sights.ClickSettings;

namespace SniperLog.Pages.Sights.ClickSettings
{
    public partial class SightClickSettingAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public SightClickSettingAddOrEditPage(SightClickSettingAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(DistanceEntry, static n => int.TryParse((string)n, out int res) && res >= 0);
            _validatorService.TryAddValidation(ElevationEntry, static n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(WindageEntry, static n => int.TryParse((string)n, out int res));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}