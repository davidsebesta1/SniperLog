using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomSwitchEntry : CustomEntryBase
{
    public static readonly BindableProperty SelectedOptionProperty = BindableProperty.Create(nameof(SelectedOption), typeof(int), typeof(Frame), 1);

    public static readonly BindableProperty LeftOptionTextProperty = BindableProperty.Create(nameof(LeftOptionText), typeof(string), typeof(Label), string.Empty);
    public static readonly BindableProperty RightOptionTextProperty = BindableProperty.Create(nameof(RightOptionText), typeof(string), typeof(Label), string.Empty);

    public CustomSwitchEntry()
    {
        InitializeComponent();
    }

    public int SelectedOption
    {
        get
        {
            return (int)GetValue(SelectedOptionProperty);
        }
        set
        {
            SetValue(SelectedOptionProperty, value);
            OnEntryInputChanged?.Invoke(this, value);
            EntryInputChangedCommand?.Execute(value);
        }
    }

    public string LeftOptionText
    {
        get
        {
            return (string)GetValue(LeftOptionTextProperty);
        }
        set
        {
            SetValue(LeftOptionTextProperty, value);
        }
    }

    public string RightOptionText
    {
        get
        {
            return (string)GetValue(RightOptionTextProperty);
        }
        set
        {
            SetValue(RightOptionTextProperty, value);
        }
    }

    private void Tapped(object sender, TappedEventArgs e)
    {
        SelectedOption = SelectedOption == 1 ? 0 : 1;
    }
}