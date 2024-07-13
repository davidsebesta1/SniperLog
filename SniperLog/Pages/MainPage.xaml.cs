using Mopups.Services;
using SniperLog.Config;
using SniperLog.Pages.Other;
using SniperLog.ViewModels;

namespace SniperLog.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000);
            VersionControl tracking = ApplicationConfigService.GetConfig<VersionControl>();
            if (tracking.FirstLaunchEver)
            {
                await MopupService.Instance.PushAsync(ServicesHelper.GetService<InitialSetupPopupPage>());
            }
        }
    }
}