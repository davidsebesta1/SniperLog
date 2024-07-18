using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Services
{
    public class ValidatorService
    {
        private readonly Dictionary<CustomEntryBase, bool> _validationValues = new Dictionary<CustomEntryBase, bool>();
        private readonly Dictionary<CustomEntryBase, Func<object, bool>> _validationFunctions = new Dictionary<CustomEntryBase, Func<object, bool>>();

        public bool AllValid => _validationValues.Values.All(x => x == true);

        public ValidatorService()
        {

        }


        public bool TryAddValidation(CustomEntryBase entry, Func<object, bool> func, bool defaultValidation = false)
        {
            if (_validationFunctions.ContainsKey(entry))
            {
                return false;
            }

            _validationValues.Add(entry, defaultValidation);
            _validationFunctions.Add(entry, func);

            entry.OnEntryInputChanged += EntryInput;

            return true;
        }

        public bool TryRemoveValidation(CustomEntryBase entry)
        {
            bool res = _validationValues.Remove(entry) && _validationFunctions.Remove(entry);

            if (res)
            {
                entry.OnEntryInputChanged -= EntryInput;
            }

            return res;
        }

        public void ClearAll()
        {
            while (_validationValues.Count > 0)
            {
                CustomEntryBase entry = _validationValues.ElementAt(0).Key;
                TryRemoveValidation(entry);
            }
        }

        private void EntryInput(object? caller, object args)
        {
            if (caller is CustomEntryBase entryBase)
            {
                bool res = _validationFunctions[entryBase].Invoke(args);
                entryBase.ErrorTextVisible = !res;

                _validationValues[entryBase] = res;
            }
        }
    }
}
