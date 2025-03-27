using SniperLog.ViewModels.Firearms.Bullets;

namespace SniperLog.Pages.Firearms.Bullets
{
    public partial class BulletsPage : ContentPage
    {
        public BulletsPage(BulletsPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as BulletsPageViewModel).RefreshCommand.ExecuteAsync(null);
        }
    }
}