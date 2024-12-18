using Mopups.Pages;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using static SniperLog.Extensions.CustomXamlComponents.ViewModels.CustomDatetimePickerPopupViewModel;

#if ANDROID
using Microsoft.Maui.Handlers;
#endif

namespace SniperLog.Extensions
{
    public partial class CustomDatetimePickerPopup : PopupPage
    {
        private TaskCompletionSource<DateTime> _taskCompletionSource;
        public Task<DateTime> PopupDismissedTask => _taskCompletionSource.Task;

        public CustomDatetimePickerPopup(CustomDatetimePickerPopupViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as CustomDatetimePickerPopupViewModel).PickedDate = DateTime.Now;
            (BindingContext as CustomDatetimePickerPopupViewModel).PickedTime = DateTime.Now.TimeOfDay;
            _taskCompletionSource = new TaskCompletionSource<DateTime>();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _taskCompletionSource.SetResult((BindingContext as CustomDatetimePickerPopupViewModel).ResultDateTime);
        }

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
            return;
        }
    }
}