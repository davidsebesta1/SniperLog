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

            _validatorService.TryAddValidation(FirearmEntry, static n => n != null);
            _validatorService.TryAddValidation(SRangeEntry, static n => n != null);
            _validatorService.TryAddValidation(SubRangeEntry, static n => n != null);
            _validatorService.TryAddValidation(AmmoEntry, static n => n != null);

            _validatorService.TryAddValidation(ElevOffsetEntry, static n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(WindOffsetEntry, static n => int.TryParse((string)n, out int res));
            _validatorService.TryAddValidation(DistanceEntry, static n => int.TryParse((string)n, out int res) && res > 0);

            RecordsPageViewModel vm = (BindingContext as RecordsPageViewModel);
            await vm.RefreshEntriesCommand.ExecuteAsync(false);

            await vm.AppearingCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}