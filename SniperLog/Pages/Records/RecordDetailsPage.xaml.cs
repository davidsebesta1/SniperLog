using SniperLog.ViewModels.Records;

namespace SniperLog.Pages.Records
{
    public partial class RecordDetailsPage : ContentPage
    {
        private ValidatorService _validatorService;
        public RecordDetailsPage(RecordDetailsPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(ElevOffsetEntry, n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(WindOffsetEntry, n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(DistanceEntry, n => int.TryParse((string)n, out int res) && res > 0);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}