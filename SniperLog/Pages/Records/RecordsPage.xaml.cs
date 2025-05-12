using AndroidX.AppCompat.App;
using Mopups.Services;
using SniperLog.Config;
using SniperLog.Pages.Other;
using SniperLog.ViewModels.Records;

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

            //First time setup
            await Task.Delay(1000);
            VersionControl tracking = ApplicationConfigService.GetConfig<VersionControl>();

            if (tracking.FirstLaunchEver)
            {
                await MopupService.Instance.PushAsync(ServicesHelper.GetService<InitialSetupPopupPage>());
                return;
            }

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