
namespace SniperLog.Services
{
    public class ValidatorService
    {
        private readonly Dictionary<Element, bool> _validationValues = new Dictionary<Element, bool>();
        private readonly Dictionary<Element, Func<object, bool>> _validationFunctions = new Dictionary<Element, Func<object, bool>>();
        private readonly Dictionary<Element, Label> _errorLabels = new Dictionary<Element, Label>();

        public bool AllValid => _validationValues.Values.All(x => x == true);

        public ValidatorService()
        {

        }

        public bool TryAddValidation(Element element, Label errorLabel, Func<object, bool> validationFunc, bool defaultValue = false)
        {
            if (_validationValues.ContainsKey(element))
            {
                return false;
            }

            switch (element)
            {
                case Entry entry:
                    entry.TextChanged += Entry_TextChanged;
                    break;
                case CheckBox checkBox:
                    checkBox.CheckedChanged += CheckBox_CheckedChanged;
                    break;
                case CollectionView collectionView:
                    collectionView.SelectionChanged += CollectionView_SelectionChanged;
                    break;
                case ListView listView:
                    listView.ItemSelected += ListView_ItemSelected;
                    break;
                default:
                    return false;
            }

            _validationValues.Add(element, defaultValue);
            _validationFunctions.Add(element, validationFunc);
            _errorLabels.Add(element, errorLabel);

            return true;
        }

        public bool RemoveValidation(Element el)
        {
            return _validationValues.Remove(el) && _validationFunctions.Remove(el) && _errorLabels.Remove(el);
        }

        public void Clear()
        {
            _validationValues.Clear();
            _validationFunctions.Clear();
            _errorLabels.Clear();
        }

        public void RevalidateAll()
        {
            for (int i = 0; i < _validationValues.Count; i++)
            {
                Element element = _validationValues.ElementAt(i).Key;

                switch (element)
                {
                    case Entry entry:
                        HandleValidationAndUpdate(element, entry.Text);
                        break;
                    case CheckBox checkBox:
                        HandleValidationAndUpdate(element, checkBox.IsChecked);
                        break;
                    case CollectionView collectionView:
                        HandleValidationAndUpdate(element, collectionView.SelectedItems);
                        break;
                    case ListView listView:
                        HandleValidationAndUpdate(element, listView.SelectedItem);
                        break;
                }
            }
        }

        private void HandleValidationAndUpdate(Element el, object args)
        {
            bool success = _validationFunctions[el].Invoke(args);

            if (_errorLabels.TryGetValue(el, out Label label) && label != null)
            {
                label.IsVisible = !success;
            }

            _validationValues[el] = success;
        }

        private void ListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            HandleValidationAndUpdate((Element)sender, e.SelectedItem);
        }

        private void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            HandleValidationAndUpdate((Element)sender, e.CurrentSelection);
        }

        private void CheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            HandleValidationAndUpdate((Element)sender, e.Value);
        }

        private void Entry_TextChanged(object? sender, TextChangedEventArgs e)
        {
            HandleValidationAndUpdate((Element)sender, e.NewTextValue);
        }
    }
}
