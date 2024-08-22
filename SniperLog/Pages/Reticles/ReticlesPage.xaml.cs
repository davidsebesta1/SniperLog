using SniperLog.ViewModels.Reticles;

namespace SniperLog.Pages.Reticles
{
    public partial class ReticlesPage : ContentPage
    {
        public ReticlesPage(ReticlesPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as ReticlesPageViewModel).RefreshReticlesCommand.ExecuteAsync(null);
        }
    }
}