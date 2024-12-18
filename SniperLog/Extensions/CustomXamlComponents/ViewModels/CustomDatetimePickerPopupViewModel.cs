using SniperLog.ViewModels;
using System.Globalization;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels
{
    public partial class CustomDatetimePickerPopupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private DateTime _pickedDate;

        [ObservableProperty]
        private TimeSpan _pickedTime;

        public DateTime ResultDateTime => new DateTime(PickedDate.Year, PickedDate.Month, PickedDate.Day, PickedTime.Hours, PickedTime.Minutes, PickedTime.Seconds);

        [ObservableProperty]
        private string _currentText;

        [ObservableProperty]
        private int _selectedOption;

        public static string TimeFormatShort => CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        partial void OnPickedDateChanged(DateTime oldValue, DateTime newValue)
        {
            if ((Options)SelectedOption == Options.Date)
                CurrentText = newValue.ToString(TimeFormatShort);
        }

        partial void OnPickedTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            if ((Options)SelectedOption == Options.Time)
                CurrentText = newValue.ToString("hh\\:mm");
        }

        partial void OnSelectedOptionChanged(int oldValue, int newValue)
        {
            if ((Options)newValue == Options.Date)
                CurrentText = PickedDate.ToString(TimeFormatShort);
            else
                CurrentText = PickedTime.ToString("hh\\:mm");
        }

        public CustomDatetimePickerPopupViewModel() : base()
        {
            PageTitle = "Pick Time and Date";
        }

        public enum Options : int
        {
            Date = 0,
            Time = 1
        }
    }
}
