namespace SniperLog.Extensions.CustomXamlComponents;

/// <summary>
/// A custom view cell for displaying data.
/// </summary>
public partial class CustomDataViewCell : ContentView
{
    /// <summary>
    /// Binding for <see cref="DataText"/>.
    /// </summary>
    public static readonly BindableProperty DataTextProperty = BindableProperty.Create(nameof(DataText), typeof(string), typeof(Label), string.Empty);

    /// <summary>
    /// Binding for <see cref="DataTextFontSize"/>.
    /// </summary>
    public static readonly BindableProperty DataTextFontSizeProperty = BindableProperty.Create(nameof(DataTextFontSize), typeof(int), typeof(Label), 28);

    /// <summary>
    /// Binding for <see cref="LabelText"/>.
    /// </summary>
    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(Label), string.Empty);

    /// <summary>
    /// Binding for <see cref="DescriptionText"/>.
    /// </summary>
    public static readonly BindableProperty DescriptionTextProperty = BindableProperty.Create(nameof(DescriptionText), typeof(string), typeof(Label), string.Empty);

    /// <inheritdoc/>
    public CustomDataViewCell()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Primary data text.
    /// </summary>
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

    /// <summary>
    /// <see cref="DataText"/> font size.
    /// </summary>
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

    /// <summary>
    /// Label text.
    /// </summary>
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

    /// <summary>
    /// Description text.
    /// </summary>
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