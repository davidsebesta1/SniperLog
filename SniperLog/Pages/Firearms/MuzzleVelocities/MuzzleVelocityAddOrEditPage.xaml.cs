using SniperLog.ViewModels.Firearms.MuzzleVelocities;
using System.Globalization;

namespace SniperLog.Pages.Firearms.MuzzleVelocities
{
    public partial class MuzzleVelocityAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public MuzzleVelocityAddOrEditPage(MuzzleVelocityAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            _validatorService.TryAddValidation(AmmunitionEntry, static n => n != null);

            _validatorService.TryAddValidation(MuzzleVelEntry, static n => string.IsNullOrEmpty((string)n) || (double.TryParse((string)n, CultureInfo.InvariantCulture, out double res) && res >= 0d));

            MuzzleVelocityAddOrEditPageViewModel vm = BindingContext as MuzzleVelocityAddOrEditPageViewModel;
            await vm.RefreshPickersCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _validatorService.ClearAll();
        }
    }
}