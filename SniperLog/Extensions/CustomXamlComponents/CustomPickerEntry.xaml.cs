using Mopups.Services;
using SniperLog.Extensions.CustomXamlComponents.Abstract;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using System.Collections;
using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents;

/// <summary>
/// A custom picker entry control that allows selecting an item from a list.
/// </summary>
public partial class CustomPickerEntry : CustomEntryBase
{
    /// <summary>
    /// The base height of the entry.
    /// </summary>
    public const int BaseHeight = 100;

    /// <summary>
    /// The height of the first row in the entry.
    /// </summary>
    public const int FirstRowBaseHeight = 30;

    /// <summary>
    /// The height of the entry row.
    /// </summary>
    public const int EntryRowBaseHeight = 55;

    /// <summary>
    /// Bindable property for <see cref="SelectedItem"/>.
    /// </summary>
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Entry), null);

    /// <summary>
    /// Bindable property for <see cref="CanSelectNone"/>.
    /// </summary>
    public static readonly BindableProperty CanSelectNoneProperty = BindableProperty.Create(nameof(CanSelectNone), typeof(bool), typeof(Entry), false);

    /// <summary>
    /// Bindable property for <see cref="SourceCollection"/>.
    /// </summary>
    public static readonly BindableProperty SourceCollectionProperty = BindableProperty.Create(nameof(SourceCollection), typeof(IList), typeof(Entry), null);

    /// <summary>
    /// Bindable property for <see cref="EntryHeight"/>.
    /// </summary>
    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    /// <summary>
    /// Bindable property for <see cref="SelectionChanged"/>.
    /// </summary>
    public static readonly BindableProperty SelectionChangedProperty = BindableProperty.Create(nameof(SelectionChanged), typeof(ICommand), typeof(Entry), null);

    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    private readonly ICommand _selectionChangedInternal;
    private CustomPickerPopup _popup;

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set
        {
            SetValue(SelectedItemProperty, value);
            if (_popup != null)
            {
                (_popup.BindingContext as CustomPickerPopupViewModel).SelectedItem = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the source collection for the picker.
    /// </summary>
    public IList SourceCollection
    {
        get => (IList)GetValue(SourceCollectionProperty);
        set => SetValue(SourceCollectionProperty, value);
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
        if (bindable is CustomPickerEntry entry)
        {
            entry.EntryHeightFinal = (int)newValue + BaseHeight;
            entry.EntryRowDefs = new RowDefinitionCollection() { new RowDefinition() { Height = FirstRowBaseHeight }, new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue } };
            entry.OnPropertyChanged(nameof(EntryHeightFinal));
            entry.OnPropertyChanged(nameof(EntryRowDefs));
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
    /// Gets or sets the row definitions for the entry.
    /// </summary>
    public RowDefinitionCollection EntryRowDefs
    {
        get => (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
        set => SetValue(EntryRowDefsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether an empty selection is allowed.
    /// </summary>
    public bool CanSelectNone
    {
        get => (bool)GetValue(CanSelectNoneProperty);
        set => SetValue(CanSelectNoneProperty, value);
    }

    /// <inheritdoc/>
    public CustomPickerEntry() : base()
    {
        InitializeComponent();
        _selectionChangedInternal = new Command(OnSelectionChangedInternal);
        _popup = new CustomPickerPopup(ServicesHelper.GetService<CustomPickerPopupViewModel>(), _selectionChangedInternal);
    }

    /// <summary>
    /// Gets or sets the command that is executed when the selection changes.
    /// </summary>
    public ICommand SelectionChanged
    {
        get => (ICommand)GetValue(SelectionChangedProperty);
        set => SetValue(SelectionChangedProperty, value);
    }

    private void OnSelectionChangedInternal(object selected)
    {
        SelectedItem = selected;
        SelectionChanged?.Execute(selected);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        CustomPickerPopupViewModel vm = (CustomPickerPopupViewModel)_popup.BindingContext;
        vm.AllCollection = SourceCollection;
        vm.CanSelectNone = CanSelectNone;
        await MopupService.Instance.PushAsync(_popup);
    }
}
