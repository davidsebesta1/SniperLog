using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLog.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels
{
    public partial class CustomPickerPopupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private IList _allCollection;

        [ObservableProperty]
        private ObservableCollection<object> _filteredCollection = new ObservableCollection<object>();

        [ObservableProperty]
        private object _selectedItem;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Visibility))]
        private bool _canSelectNone;

        public Visibility Visibility => !CanSelectNone ? Visibility.Visible : Visibility.Hidden;

        public CustomPickerPopupViewModel() : base()
        {

        }

        [RelayCommand]
        private async Task SelectNone()
        {
            SelectedItem = null;
            await MopupService.Instance.PopAsync();
        }

        [RelayCommand]
        private async Task Search(string value)
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
    }
}