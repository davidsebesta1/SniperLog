using Microsoft.Maui.Handlers;
using System.Globalization;
using System.Windows.Input;
using YamlDotNet.Core.Tokens;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomDateSearchBar : ContentView
{
    #region Bindables

    private static readonly BindableProperty StringValProperty = BindableProperty.Create(nameof(StringVal), typeof(string), typeof(Frame), TimeFormatShort);
    private static readonly BindableProperty TextColorValProperty = BindableProperty.Create(nameof(TextColorVal), typeof(Color), typeof(Frame), (Color)Application.Current.Resources["TextGrey"]);

    public static readonly BindableProperty DateValueProperty = BindableProperty.Create(nameof(DateValue), typeof(DateTime?), typeof(Frame), null);
    public static readonly BindableProperty EnterCommandProperty = BindableProperty.Create(nameof(EnterCommand), typeof(ICommand), typeof(Entry), null);

    #endregion

    #region Properties

    public static string TimeFormatShort => CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.Replace("m", "mm", StringComparison.InvariantCultureIgnoreCase).Replace("d", "dd", StringComparison.InvariantCultureIgnoreCase);

    public Color TextColorVal
    {
        get => (Color)GetValue(TextColorValProperty);
        set => SetValue(TextColorValProperty, value);
    }

    public string StringVal
    {
        get => (string)GetValue(StringValProperty);
        set
        {
            SetValue(StringValProperty, value);
            TextColorVal = (value == TimeFormatShort) ? (Color)Application.Current.Resources["TextGrey"] : (Color)Application.Current.Resources["Text"];
        }
    }

    public DateTime? DateValue
    {
        get => (DateTime?)GetValue(DateValueProperty);
        set
        {
            SetValue(DateValueProperty, value);
            StringVal = DateValue?.ToString("d") ?? TimeFormatShort;
        }
    }

    public ICommand EnterCommand
    {
        get => (ICommand)GetValue(EnterCommandProperty);
        set => SetValue(EnterCommandProperty, value);
    }

    #endregion

    #region Ctro

    public CustomDateSearchBar()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (EnterCommand != null && EnterCommand.CanExecute(e.NewDate))
        {
            EnterCommand.Execute(e.NewDate);
        }
    }

    private void ImageButton_Pressed(object sender, EventArgs e)
    {
        DateValue = null;
        if (EnterCommand != null && EnterCommand.CanExecute(string.Empty))
        {
            EnterCommand.Execute(string.Empty);
        }
    }

    #endregion

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