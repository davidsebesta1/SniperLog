using SniperLog.ViewModels.SRanges.Subranges;

namespace SniperLog.Pages.ShootingRanges.Subranges
{
    public partial class SubRangeAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;
        private readonly DataCacherService<SubRange> _cacher;

        public SubRangeAddOrEditPage(SubRangeAddOrEditPageViewModel vm, ValidatorService validatorService, DataCacherService<SubRange> cacher)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
            _cacher = cacher;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(PrefixEntry, (obj) => ((string)obj).Length == 1 && !(_cacher.GetAllBy(n => n.ShootingRange_ID == (BindingContext as SubRangeAddOrEditPageViewModel).OwningRange.ID && ((BindingContext as SubRangeAddOrEditPageViewModel).Subrange == null || n.ID != (BindingContext as SubRangeAddOrEditPageViewModel).Subrange.ID))).GetAwaiter().GetResult().Any(n => n.Prefix.ToString() == (string)obj));
            _validatorService.TryAddValidation(RangeEntry, static (obj) => double.TryParse((string)obj, out double val) && val >= 0d);
            _validatorService.TryAddValidation(AltEntry, static (obj) => string.IsNullOrEmpty((string)obj) || double.TryParse((string)obj, out double val));
            _validatorService.TryAddValidation(FiringDirEntry, static (obj) => string.IsNullOrEmpty((string)obj) || double.TryParse((string)obj, out double val) && val >= 0d && val <= 360d);
            _validatorService.TryAddValidation(VerticalDirEntry, static (obj) => string.IsNullOrEmpty((string)obj) || double.TryParse((string)obj, out double val));

            await (BindingContext as SubRangeAddOrEditPageViewModel).RefreshEntriesCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}