using Mopups.Pages;
using Mopups.Services;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using SniperLog.ViewModels;
using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents
{
    public partial class CustomPickerPopup : PopupPage
    {
        private readonly ICommand _selectionChangedCommand;

        public CustomPickerPopup(CustomPickerPopupViewModel vm, ICommand selectionChangedCommand)
        {
            InitializeComponent();
            BindingContext = vm;
            _selectionChangedCommand = selectionChangedCommand;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as CustomPickerPopupViewModel).SearchCommand.ExecuteAsync(string.Empty);
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectionChangedCommand.Execute(e.SelectedItem);
            await MopupService.Instance.PopAsync();
        }
    }
}