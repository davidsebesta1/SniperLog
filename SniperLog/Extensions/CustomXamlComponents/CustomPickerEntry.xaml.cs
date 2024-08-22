using Mopups.Services;
using SniperLog.Extensions.CustomXamlComponents.Abstract;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using System.Collections;
using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomPickerEntry : CustomEntryBase
{
    public const int BaseHeight = 100;
    public const int FirstRowBaseHeight = 30;
    public const int EntryRowBaseHeight = 55;

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Entry), null);

    public static readonly BindableProperty SourceCollectionProperty = BindableProperty.Create(nameof(SelectedItem), typeof(IList), typeof(Entry), null);

    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    public static readonly BindableProperty SelectionChangedProperty = BindableProperty.Create(nameof(SelectionChanged), typeof(ICommand), typeof(Entry), null);

    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    public object SelectedItem
    {
        get
        {
            return GetValue(SelectedItemProperty);
        }
        set
        {
            SetValue(SelectedItemProperty, value);
            if (_popup != null)
            {
                (_popup.BindingContext as CustomPickerPopupViewModel).SelectedItem = value;
            }
        }
    }

    public IList SourceCollection
    {
        get => (IList)GetValue(SourceCollectionProperty);
        set => SetValue(SourceCollectionProperty, value);
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
        if (bindable is CustomPickerEntry entry)
        {
            entry.EntryHeightFinal = (int)newValue + BaseHeight;
            entry.EntryRowDefs = new RowDefinitionCollection() { new RowDefinition() { Height = FirstRowBaseHeight }, new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue } };
            entry.OnPropertyChanged(nameof(EntryHeightFinal));
            entry.OnPropertyChanged(nameof(EntryRowDefs));
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

    private CustomPickerPopup _popup;

    public CustomPickerEntry() : base()
    {
        InitializeComponent();

        _selectionChangedInternal = new Command(OnSelectionChangedInternal);
        _popup = new CustomPickerPopup(ServicesHelper.GetService<CustomPickerPopupViewModel>(), _selectionChangedInternal);
    }

    public ICommand SelectionChanged
    {
        get => (ICommand)GetValue(SelectionChangedProperty);
        set => SetValue(SelectionChangedProperty, value);
    }

    private readonly ICommand _selectionChangedInternal;

    private async void OnSelectionChangedInternal(object selected)
    {
        SelectedItem = selected;
        SelectionChanged?.Execute(selected);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        (_popup.BindingContext as CustomPickerPopupViewModel).AllCollection = SourceCollection;
        await MopupService.Instance.PushAsync(_popup);
    }
}