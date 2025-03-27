using SniperLog.ViewModels.Firearms.MuzzleVelocities;

namespace SniperLog.Pages.Firearms.MuzzleVelocities
{
    public partial class MuzzleVelocitiesPage : ContentPage
    {
        public MuzzleVelocitiesPage(MuzzleVelocitiesPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MuzzleVelocitiesPageViewModel vm = BindingContext as MuzzleVelocitiesPageViewModel;
            await vm.SearchCommand.ExecuteAsync(null);
        }
    }
}