using SniperLog.ViewModels.SRanges.Subranges;

namespace SniperLog.Pages.ShootingRanges.Subranges
{
    public partial class SubRangeAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public SubRangeAddOrEditPage(SubRangeAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(PrefixEntry, (obj) => ((string)obj).Length == 1);
            _validatorService.TryAddValidation(RangeEntry, (obj) => double.TryParse((string)obj, out double val) && val >= 0d);
            _validatorService.TryAddValidation(AltEntry, (obj) => string.IsNullOrEmpty((string)obj) || double.TryParse((string)obj, out double val));
            _validatorService.TryAddValidation(FiringDirEntry, (obj) => string.IsNullOrEmpty((string)obj) || double.TryParse((string)obj, out double val) && val >= 0d && val <= 360d);
            _validatorService.TryAddValidation(VerticalDirEntry, (obj) => string.IsNullOrEmpty((string)obj) || double.TryParse((string)obj, out double val));

            await (BindingContext as SubRangeAddOrEditPageViewModel).RefreshEntriesCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}