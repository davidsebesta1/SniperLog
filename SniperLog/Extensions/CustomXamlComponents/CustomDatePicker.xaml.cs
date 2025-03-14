using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents;

/// <summary>
/// A custom date picker component.
/// </summary>
public partial class CustomDatePicker : CustomEntryBase
{
    /// <summary>
    /// Bindable property for <see cref="DateValue"/>.
    /// </summary>
    public static readonly BindableProperty DateValueProperty = BindableProperty.Create(nameof(DateValue), typeof(DateTime), typeof(Entry), DateTime.Now);

    /// <summary>
    /// Bindable property for <see cref="EntryHeight"/>.
    /// </summary>
    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    /// <summary>
    /// Bindable property for <see cref="EntryHeightFinal"/>.
    /// </summary>
    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);

    /// <summary>
    /// Bindable property for <see cref="EntryRowDefs"/>.
    /// </summary>
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    /// <summary>
    /// Base height of the entry component.
    /// </summary>
    public const int BaseHeight = 100;

    /// <summary>
    /// Base height for the first row.
    /// </summary>
    public const int FirstRowBaseHeight = 30;

    /// <summary>
    /// Base height for the entry row.
    /// </summary>
    public const int EntryRowBaseHeight = 55;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDatePicker"/> class.
    /// </summary>
    public CustomDatePicker()
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

    /// <summary>
    /// Callback method triggered when <see cref="EntryHeight"/> changes.
    /// </summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnEntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomDatePicker customTextEntry)
        {
            customTextEntry.EntryHeightFinal = (int)newValue + BaseHeight;
            customTextEntry.EntryRowDefs = new RowDefinitionCollection
            {
                new RowDefinition { Height = FirstRowBaseHeight },
                new RowDefinition { Height = EntryRowBaseHeight + (int)newValue }
            };

            customTextEntry.OnPropertyChanged(nameof(EntryHeightFinal));
            customTextEntry.OnPropertyChanged(nameof(EntryRowDefs));
        }
    }

    /// <summary>
    /// Gets or sets the final computed height of the entry.
    /// </summary>
    public int EntryHeightFinal
    {
        get => (int)GetValue(EntryHeightFinalProperty);
        set => SetValue(EntryHeightFinalProperty, value);
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
    /// Gets or sets the selected date value.
    /// </summary>
    public DateTime DateValue
    {
        get => (DateTime)GetValue(DateValueProperty);
        set
        {
            SetValue(DateValueProperty, value);
            OnEntryInputChanged?.Invoke(this, value);
            EntryInputChangedCommand?.Execute(value);
        }
    }
}
