using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLog.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels;

/// <summary>
/// <see cref="CustomPickerPopup"/>'s viewmodel.
/// </summary>
public partial class CustomPickerPopupViewModel : BaseViewModel
{
    /// <summary>
    /// All of the available items.
    /// </summary>
    [ObservableProperty]
    private IList _allCollection;

    /// <summary>
    /// Actual collection that is visible in the <see cref="CollectionView"/>.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<object> _filteredCollection = new ObservableCollection<object>();

    /// <summary>
    /// The item that is selected.
    /// </summary>
    [ObservableProperty]
    private object _selectedItem;

    /// <summary>
    /// Whether the user can select nothing.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectNoneVisiblity))]
    private bool _canSelectNone;

    /// <summary>
    /// Search text binded property.
    /// </summary>
    [ObservableProperty]
    private string _searchText;

    /// <summary>
    /// The command used in <see cref="CustomPickerEntry"/>.
    /// </summary>
    public ICommand SelectionChangedCommandEntry { get; set; }

    /// <summary>
    /// Gets whether the select none button should be visible.
    /// </summary>
    public Visibility SelectNoneVisiblity => !CanSelectNone ? Visibility.Visible : Visibility.Hidden;

    /// <inheritdoc/>
    public CustomPickerPopupViewModel() : base()
    {

    }

    /// <summary>
    /// Sets <see cref="SelectedItem"/> to <see langword="null"/>.
    /// </summary>
    [RelayCommand]
    private async Task SelectNone()
    {
        SelectedItem = null;
        await MopupService.Instance.PopAsync();
    }

    /// <summary>
    /// Searches the <see cref="AllCollection"/> and filters by the parameter.
    /// </summary>
    /// <param name="value">The target value to search for in objects <see cref="object.ToString()"/> or its override.</param>
    [RelayCommand]
    private void Search(string value)
    {
        FilteredCollection.Clear();

        if (string.IsNullOrEmpty(value))
        {
            foreach (object item in AllCollection)
            {
                FilteredCollection.Add(item);
            }

            return;
        }

        foreach (object item in AllCollection)
        {
            if (item.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase) || SelectedItem == item)
            {
                FilteredCollection.Add(item);
            }
        }
    }

    /// <summary>
    /// Closes the popup.
    /// </summary>
    [RelayCommand]
    private async Task Close()
    {
        await MopupService.Instance.PopAsync();
    }

    /// <summary>
    /// Command that to execute the base <see cref="CustomPickerEntry"/> selection changed command.
    /// </summary>
    [RelayCommand]
    private async Task SelectionChanged()
    {
        SelectionChangedCommandEntry.Execute(SelectedItem);
        await MopupService.Instance.PopAsync();
    }
}
