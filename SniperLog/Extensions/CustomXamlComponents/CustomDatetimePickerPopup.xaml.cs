using Mopups.Pages;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using static SniperLog.Extensions.CustomXamlComponents.ViewModels.CustomDatetimePickerPopupViewModel;

#if ANDROID
using Microsoft.Maui.Handlers;
#endif

namespace SniperLog.Extensions
{
    /// <summary>
    /// A custom popup for selecting a date and time.
    /// </summary>
    public partial class CustomDatetimePickerPopup : PopupPage
    {
        private TaskCompletionSource<DateTime> _taskCompletionSource;

        /// <summary>
        /// Gets a task that completes when the popup is dismissed, returning the selected date and time.
        /// </summary>
        public Task<DateTime> PopupDismissedTask => _taskCompletionSource.Task;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDatetimePickerPopup"/> class.
        /// </summary>
        /// <param name="vm">The view model for the popup.</param>
        public CustomDatetimePickerPopup(CustomDatetimePickerPopupViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        /// <summary>
        /// Called when the popup appears. Initializes the selected date and time.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            CustomDatetimePickerPopupViewModel vm = BindingContext as CustomDatetimePickerPopupViewModel;
            vm.PickedDate = DateTime.Now;
            vm.PickedTime = DateTime.Now.TimeOfDay;
            _taskCompletionSource = new TaskCompletionSource<DateTime>();
        }

        /// <summary>
        /// Called when the popup disappears. Sets the result date and time.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _taskCompletionSource.SetResult((BindingContext as CustomDatetimePickerPopupViewModel).ResultDateTime);
        }

        /// <summary>
        /// Handles the tap gesture event to open the date or time picker.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The tap event arguments.</param>
        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if ((Options)SwitchEntry.SelectedOption == Options.Date)
            {
#if ANDROID
                IDatePickerHandler handler = DatePicker.Handler as IDatePickerHandler;
                handler.PlatformView.PerformClick();
#else
                DatePicker.Focus();
#endif
                return;
            }

#if ANDROID
            ITimePickerHandler handler2 = TimePicker.Handler as ITimePickerHandler;
            handler2.PlatformView.PerformClick();
#else
            TimePicker.Focus();
#endif
        }
    }
}
