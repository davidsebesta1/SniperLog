using Mopups.Pages;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents
{
    /// <summary>
    /// A custom popup page for selecting an item from a picker.
    /// </summary>
    public partial class CustomPickerPopup : PopupPage
    {
        private readonly ICommand _selectionChangedCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPickerPopup"/> class.
        /// </summary>
        /// <param name="vm">The view model for the popup.</param>
        /// <param name="selectionChangedCommand">The command to execute when a selection is made.</param>
        public CustomPickerPopup(CustomPickerPopupViewModel vm, ICommand selectionChangedCommand)
        {
            InitializeComponent();
            BindingContext = vm;
            _selectionChangedCommand = selectionChangedCommand;
        }

        /// <summary>
        /// Called when the popup page is appearing.
        /// Sets up the selection command and clears the search text.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            CustomPickerPopupViewModel vm = (CustomPickerPopupViewModel)BindingContext;

            vm.SelectionChangedCommandEntry = _selectionChangedCommand;
            vm.SearchText = string.Empty;
            vm.SearchCommand.Execute(string.Empty);
            vm.SelectedItem = null;
        }
    }
}
