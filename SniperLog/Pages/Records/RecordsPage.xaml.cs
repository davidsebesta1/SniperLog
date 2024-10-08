using SniperLog.ViewModels.Records;
using System.Reflection;
using System.Xml.Linq;

namespace SniperLog.Pages.Records
{
    public partial class RecordsPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public RecordsPage(RecordsPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
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
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}