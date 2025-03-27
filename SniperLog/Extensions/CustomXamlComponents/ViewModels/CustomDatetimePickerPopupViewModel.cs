using SniperLog.ViewModels;
using System.Globalization;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels;

/// <summary>
/// <see cref="CustomDatetimePickerPopup"/>'s viewmodel.
/// </summary>
public partial class CustomDatetimePickerPopupViewModel : BaseViewModel
{
    /// <summary>
    /// Picked date.
    /// </summary>
    [ObservableProperty]
    private DateTime _pickedDate;

    /// <summary>
    /// Picked time.
    /// </summary>
    [ObservableProperty]
    private TimeSpan _pickedTime;

    /// <summary>
    /// Combination of <see cref="PickedDate"/> and <see cref="PickedTime"/>.
    /// </summary>
    public DateTime ResultDateTime => new DateTime(PickedDate.Year, PickedDate.Month, PickedDate.Day, PickedTime.Hours, PickedTime.Minutes, PickedTime.Seconds);

    /// <summary>
    /// Current text.
    /// </summary>
    [ObservableProperty]
    private string _currentText;

    /// <summary>
    /// Selected option.
    /// </summary>
    [ObservableProperty]
    private int _selectedOption;

    /// <summary>
    /// Time format used for displaying date.
    /// </summary>
    public static string TimeFormatShort => CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    /// <inheritdoc/>
    partial void OnPickedDateChanged(DateTime oldValue, DateTime newValue)
    {
        if ((Options)SelectedOption == Options.Date)
            CurrentText = newValue.ToString(TimeFormatShort);
    }

    /// <inheritdoc/>
    partial void OnPickedTimeChanged(TimeSpan oldValue, TimeSpan newValue)
    {
        if ((Options)SelectedOption == Options.Time)
            CurrentText = newValue.ToString("hh\\:mm");
    }

    /// <inheritdoc/>
    partial void OnSelectedOptionChanged(int oldValue, int newValue)
    {
        if ((Options)newValue == Options.Date)
            CurrentText = PickedDate.ToString(TimeFormatShort);
        else
            CurrentText = PickedTime.ToString("hh\\:mm");
    }

    /// <inheritdoc/>
    public CustomDatetimePickerPopupViewModel() : base()
    {
        PageTitle = "Pick Time and Date";
    }

    /// <summary>
    /// Options for editing time or date.
    /// </summary>
    public enum Options : int
    {
        Date = 0,
        Time = 1
    }
}

