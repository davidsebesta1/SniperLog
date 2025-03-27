using SniperLog.ViewModels.Other;

namespace SniperLog.Pages.Other
{
    public partial class ImportExportPage : ContentPage
    {
        public ImportExportPage(ImportExportPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as ImportExportPageViewModel).LoadFirearmsCommand.ExecuteAsync(null);
        }
    }
}