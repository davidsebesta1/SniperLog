using SniperLog.ViewModels.Reticles;

namespace SniperLog.Pages.Reticles
{
    public partial class ReticleAddOrEditPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public ReticleAddOrEditPage(ReticleAddOrEditPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}