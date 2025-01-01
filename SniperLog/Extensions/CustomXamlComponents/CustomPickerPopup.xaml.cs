using Mopups.Pages;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
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
            CustomPickerPopupViewModel vm = (CustomPickerPopupViewModel)BindingContext;

            vm.SelectionChangedCommandEntry = _selectionChangedCommand;
            vm.SearchCommand.Execute(string.Empty);
        }
    }
}