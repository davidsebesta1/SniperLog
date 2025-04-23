using Mopups.Pages;
using Mopups.Services;
using SniperLog.Pages.Records;
using SniperLog.ViewModels.Other;

namespace SniperLog.Pages.Other
{
    public partial class InitialSetupPopupPage : PopupPage
    {
        public InitialSetupPopupPage(InitialSetupPopupPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                InitialSetupPopupPageViewModel vm = (InitialSetupPopupPageViewModel)BindingContext;

                await Task.Delay(1000);
                ProgressBar.Progress = 0d;

                InfoLabel.Text = "Performing first time setup...\nSetuping local SQLite database..";
                await vm.LoadInitialDatabaseCommand.ExecuteAsync(null);

                ProgressBar.Progress = 0.5d;
                await Task.Delay(1000);
                InfoLabel.Text = "Performing first time setup...\nInitializing base database values..";

                await vm.LoadInitialDataCommand.ExecuteAsync(null);

                ProgressBar.Progress = 1d;
                InfoLabel.Text = "Performing first time setup...\nDone!";
                await Task.Delay(1000);

                vm.FinalizeInitialLoadCommand.Execute(null);

                await MopupService.Instance.PopAsync();
                await Shell.Current.GoToAsync(nameof(RecordsPage));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("FATAL ERROR, PLEASE CONTACT DEVELOPER", ex.ToString(), "Okay");
            }
        }
    }
}