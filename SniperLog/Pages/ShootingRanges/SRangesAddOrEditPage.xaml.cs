using SniperLog.Extensions.CustomXamlComponents.Abstract;
using SniperLog.ViewModels.SRanges;

namespace SniperLog.Pages.ShootingRanges
{
    public partial class SRangesAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public SRangesAddOrEditPage(SRangesAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;

            _validatorService = validatorService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(NameEntry, static (obj) => !string.IsNullOrEmpty((string)obj));
            _validatorService.TryAddValidation(LatEntry, static (obj) => string.IsNullOrEmpty((string)obj) || (double.TryParse((string)obj, out double val) && Math.Abs(val) <= 90d));
            _validatorService.TryAddValidation(LongEntry, static (obj) => string.IsNullOrEmpty((string)obj) || (double.TryParse((string)obj, out double val) && Math.Abs(val) <= 180d));

            LocationTypeEntry.OnEntryInputChanged += LocationTypeInputChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LocationTypeEntry.OnEntryInputChanged -= LocationTypeInputChanged;

            _validatorService.ClearAll();
        }

        private void LocationTypeInputChanged(object? caller, object args)
        {
            bool value = (int)args == 0;

            LatEntry.IsEnabled = value;
            LongEntry.IsEnabled = value;
            AddressEntry.IsEnabled = !value;
        }
    }
}