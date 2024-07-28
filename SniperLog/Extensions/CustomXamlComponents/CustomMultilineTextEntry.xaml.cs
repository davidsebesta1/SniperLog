using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomMultilineTextEntry : CustomEntryBase
{
    public static readonly BindableProperty TextValueProperty = BindableProperty.Create(nameof(TextValue), typeof(string), typeof(Entry), string.Empty);

    public static readonly BindableProperty PlaceholderTextValueProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(Entry), string.Empty);

    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    public static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    public static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);
    public static readonly BindableProperty EditorHeightProperty = BindableProperty.Create(nameof(EditorHeight), typeof(int), typeof(Grid), null);

    public const int BaseHeight = 100;
    public const int FirstRowBaseHeight = 30;
    public const int EntryRowBaseHeight = 55;

    public CustomMultilineTextEntry()
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

    public int EditorHeight
    {
        get
        {
            return (int)GetValue(EditorHeightProperty);
        }
        set
        {
            SetValue(EditorHeightProperty, value);
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

    public string TextValue
    {
        get
        {
            return (string)GetValue(TextValueProperty);
        }
        set
        {
            SetValue(TextValueProperty, value);
            OnEntryInputChanged?.Invoke(this, value);
            EntryInputChangedCommand?.Execute(value);
        }
    }

    public string PlaceholderText
    {
        get
        {
            return (string)GetValue(PlaceholderTextValueProperty);
        }
        set
        {
            SetValue(PlaceholderTextValueProperty, value);
        }
    }
}