using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomDatePicker : CustomEntryBase
{
    public static readonly BindableProperty DateValueProperty = BindableProperty.Create(nameof(DateValue), typeof(DateTime), typeof(Entry), DateTime.Now);

    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    public const int BaseHeight = 100;
    public const int FirstRowBaseHeight = 30;
    public const int EntryRowBaseHeight = 55;

    public CustomDatePicker()
    {
        InitializeComponent();
    }

    public int EntryHeight
    {
        get
        {
            return (int)GetValue(EntryHeightProperty);
        }
        set
        {
            SetValue(EntryHeightProperty, value);
        }
    }

    private static void OnEntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomDatePicker customTextEntry)
        {
            customTextEntry.EntryHeightFinal = (int)newValue + BaseHeight;
            customTextEntry.EntryRowDefs = new RowDefinitionCollection() { new RowDefinition() { Height = FirstRowBaseHeight }, new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue } };
            customTextEntry.OnPropertyChanged(nameof(EntryHeightFinal));
            customTextEntry.OnPropertyChanged(nameof(EntryRowDefs));
        }
    }

    public int EntryHeightFinal
    {
        get
        {
            return (int)GetValue(EntryHeightFinalProperty);
        }
        set
        {
            SetValue(EntryHeightFinalProperty, value);
        }
    }

    public RowDefinitionCollection EntryRowDefs
    {
        get
        {
            return (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
        }
        set
        {
            SetValue(EntryRowDefsProperty, value);
        }
    }

    public DateTime DateValue
    {
        get
        {
            return (DateTime)GetValue(DateValueProperty);
        }
        set
        {
            SetValue(DateValueProperty, value);
            OnEntryInputChanged?.Invoke(this, value);
            EntryInputChangedCommand?.Execute(value);
        }
    }
}