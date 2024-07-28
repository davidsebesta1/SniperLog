namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomDataViewCell : ContentView
{
    public static readonly BindableProperty DataTextProperty = BindableProperty.Create(nameof(DataText), typeof(string), typeof(Label), string.Empty);
    public static readonly BindableProperty DataTextFontSizeProperty = BindableProperty.Create(nameof(DataTextFontSize), typeof(int), typeof(Label), 28);

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(Label), string.Empty);
    public static readonly BindableProperty DescriptionTextProperty = BindableProperty.Create(nameof(DescriptionText), typeof(string), typeof(Label), string.Empty);

    public CustomDataViewCell()
    {
        InitializeComponent();
    }

    public string DataText
    {
        get
        {
            return (string)GetValue(DataTextProperty);
        }
        set
        {
            SetValue(DataTextProperty, value);
        }
    }

    public int DataTextFontSize
    {
        get
        {
            return (int)GetValue(DataTextFontSizeProperty);
        }
        set
        {
            SetValue(DataTextFontSizeProperty, value);
        }
    }

    public string LabelText
    {
        get
        {
            return (string)GetValue(LabelTextProperty);
        }
        set
        {
            SetValue(LabelTextProperty, value);
        }
    }

    public string DescriptionText
    {
        get
        {
            return (string)GetValue(DescriptionTextProperty);
        }
        set
        {
            SetValue(DescriptionTextProperty, value);
        }
    }
}