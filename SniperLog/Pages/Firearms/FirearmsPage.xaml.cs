using SniperLog.ViewModels.Firearms;

namespace SniperLog.Pages.Firearms
{
    public partial class FirearmsPage : ContentPage
    {
        public FirearmsPage(FirearmsPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as FirearmsPageViewModel).RefreshCommand.ExecuteAsync(null);
        }
    }
}