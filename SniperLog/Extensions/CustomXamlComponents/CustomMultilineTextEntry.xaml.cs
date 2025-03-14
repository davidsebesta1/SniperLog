using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents;

/// <summary>
/// A custom multiline text entry control.
/// </summary>
public partial class CustomMultilineTextEntry : CustomEntryBase
{
    /// <summary>
    /// Bindable property for the <see cref="TextValue"/> of the entry.
    /// </summary>
    public static readonly BindableProperty TextValueProperty = BindableProperty.Create(nameof(TextValue), typeof(string), typeof(Entry), string.Empty);

    /// <summary>
    /// Bindable property for the <see cref="PlaceholderText"/> of the entry.
    /// </summary>
    public static readonly BindableProperty PlaceholderTextValueProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(Entry), string.Empty);

    /// <summary>
    /// Bindable property for the <see cref="EntryHeight"/>.
    /// </summary>
    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    /// <summary>
    /// Bindable property for the <see cref="EntryHeightFinal"/> of the entry.
    /// </summary>
    public static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);

    /// <summary>
    /// Bindable property for <see cref="EntryRowDefs"/> of the entry.
    /// </summary>
    public static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    /// <summary>
    /// Bindable property for the <see cref="EditorHeight"/>.
    /// </summary>
    public static readonly BindableProperty EditorHeightProperty = BindableProperty.Create(nameof(EditorHeight), typeof(int), typeof(Grid), null);

    /// <summary>
    /// Base height of the entry.
    /// </summary>
    public const int BaseHeight = 100;

    /// <summary>
    /// Height of the first row in the entry.
    /// </summary>
    public const int FirstRowBaseHeight = 30;

    /// <summary>
    /// Height of the entry row.
    /// </summary>
    public const int EntryRowBaseHeight = 55;

    /// <inheritdoc/>
    public CustomMultilineTextEntry() : base()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the height of the entry.
    /// </summary>
    public int EntryHeight
    {
        get => (int)GetValue(EntryHeightProperty);
        set => SetValue(EntryHeightProperty, value);
    }

    private static void OnEntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomMultilineTextEntry customTextEntry)
        {
            customTextEntry.EntryHeightFinal = (int)newValue + BaseHeight;
            customTextEntry.EntryRowDefs = new RowDefinitionCollection() { new RowDefinition() { Height = FirstRowBaseHeight }, new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue } };
            customTextEntry.EditorHeight = EntryRowBaseHeight + (int)newValue;
            customTextEntry.OnPropertyChanged(nameof(EntryHeightFinal));
            customTextEntry.OnPropertyChanged(nameof(EntryRowDefs));
            customTextEntry.OnPropertyChanged(nameof(EditorHeight));
        }
    }

    /// <summary>
    /// Gets or sets the final calculated height of the entry.
    /// </summary>
    public int EntryHeightFinal
    {
        get => (int)GetValue(EntryHeightFinalProperty);
        set => SetValue(EntryHeightFinalProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the editor.
    /// </summary>
    public int EditorHeight
    {
        get => (int)GetValue(EditorHeightProperty);
        set => SetValue(EditorHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the row definitions for the entry.
    /// </summary>
    public RowDefinitionCollection EntryRowDefs
    {
        get => (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
        set => SetValue(EntryRowDefsProperty, value);
    }

    /// <summary>
    /// Gets or sets the text value of the entry.
    /// </summary>
    public string TextValue
    {
        get => (string)GetValue(TextValueProperty);
        set
        {
            SetValue(TextValueProperty, value);
            OnEntryInputChanged?.Invoke(this, value);
            EntryInputChangedCommand?.Execute(value);
        }
    }

    /// <summary>
    /// Gets or sets the placeholder text of the entry.
    /// </summary>
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextValueProperty);
        set => SetValue(PlaceholderTextValueProperty, value);
    }
}
