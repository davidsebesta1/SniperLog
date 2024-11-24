using SniperLog.ViewModels.Records;

namespace SniperLog.Pages.Records
{
    public partial class RecordsPage : ContentPage
    {
        private readonly ValidatorService _validatorService;
        private readonly DataCacherService<Firearm> _firearmCacher;

        public RecordsPage(RecordsPageViewModel vm, ValidatorService validatorService, DataCacherService<Firearm> firearmCacher)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
            _firearmCacher = firearmCacher;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(FirearmEntry, n => n != null);
            _validatorService.TryAddValidation(SRangeEntry, n => n != null);
            _validatorService.TryAddValidation(SubRangeEntry, n => n != null);

            _validatorService.TryAddValidation(ElevOffsetEntry, n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(WindOffsetEntry, n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(DistanceEntry, n => int.TryParse((string)n, out int res) && res > 0);

            await (BindingContext as RecordsPageViewModel).RefreshEntriesCommand.ExecuteAsync(false);

            Firearm first = (await _firearmCacher.GetAll()).First();
            (BindingContext as RecordsPageViewModel).SelectedFirearm = first;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}