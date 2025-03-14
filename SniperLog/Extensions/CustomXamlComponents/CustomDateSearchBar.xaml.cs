using System.Globalization;
using System.Windows.Input;

#if ANDROID
using Microsoft.Maui.Handlers;
#endif

namespace SniperLog.Extensions.CustomXamlComponents;

/// <summary>
/// A custom search bar with an integrated date picker.
/// </summary>
public partial class CustomDateSearchBar : ContentView
{
    #region Bindables

    /// <summary>
    /// Bindable property for <see cref="StringVal"/>.
    /// </summary>
    private static readonly BindableProperty StringValProperty = BindableProperty.Create(nameof(StringVal), typeof(string), typeof(Frame), TimeFormatShort);

    /// <summary>
    /// Bindable property for <see cref="TextColorVal"/>.
    /// </summary>
    private static readonly BindableProperty TextColorValProperty = BindableProperty.Create(nameof(TextColorVal), typeof(Color), typeof(Frame), (Color)Application.Current.Resources["TextGrey"]);

    /// <summary>
    /// Bindable property for <see cref="DateValue"/>.
    /// </summary>
    public static readonly BindableProperty DateValueProperty = BindableProperty.Create(nameof(DateValue), typeof(DateTime?), typeof(Frame), null);

    /// <summary>
    /// Bindable property for <see cref="EnterCommand"/>.
    /// </summary>
    public static readonly BindableProperty EnterCommandProperty = BindableProperty.Create(nameof(EnterCommand), typeof(ICommand), typeof(Entry), null);

    #endregion

    #region Properties

    /// <summary>
    /// Gets the short date format string based on the current culture.
    /// </summary>
    public static string TimeFormatShort => CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
        .Replace("m", "mm", StringComparison.InvariantCultureIgnoreCase)
        .Replace("d", "dd", StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Gets or sets the text color of the search bar.
    /// </summary>
    public Color TextColorVal
    {
        get => (Color)GetValue(TextColorValProperty);
        set => SetValue(TextColorValProperty, value);
    }

    /// <summary>
    /// Gets or sets the string representation of the selected date.
    /// </summary>
    public string StringVal
    {
        get => (string)GetValue(StringValProperty);
        set
        {
            SetValue(StringValProperty, value);
            TextColorVal = (value == TimeFormatShort) ? (Color)Application.Current.Resources["TextGrey"]  : (Color)Application.Current.Resources["Text"];
        }
    }

    /// <summary>
    /// Gets or sets the selected date value.
    /// </summary>
    public DateTime? DateValue
    {
        get => (DateTime?)GetValue(DateValueProperty);
        set
        {
            SetValue(DateValueProperty, value);
            StringVal = DateValue?.ToString("d") ?? TimeFormatShort;
        }
    }

    /// <summary>
    /// Gets or sets the command executed when the enter key is pressed.
    /// </summary>
    public ICommand EnterCommand
    {
        get => (ICommand)GetValue(EnterCommandProperty);
        set => SetValue(EnterCommandProperty, value);
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDateSearchBar"/> class.
    /// </summary>
    public CustomDateSearchBar()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    /// <summary>
    /// Handles the event when a date is selected in the date picker.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Date selection event arguments.</param>
    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (EnterCommand != null && EnterCommand.CanExecute(e.NewDate))
        {
            EnterCommand.Execute(e.NewDate);
        }
    }

    /// <summary>
    /// Handles the event when the clear button is pressed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void ImageButton_Pressed(object sender, EventArgs e)
    {
        DateValue = null;
        if (EnterCommand != null && EnterCommand.CanExecute(null))
        {
            EnterCommand.Execute(null);
        }
    }

    #endregion

    /// <summary>
    /// Handles the tap gesture event to open the date picker.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Tap event arguments.</param>
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
#if ANDROID
        var handler = DatePickerEntry.Handler as IDatePickerHandler;
        handler.PlatformView.PerformClick();
#else
        DatePickerEntry.Focus();
#endif
    }
}
