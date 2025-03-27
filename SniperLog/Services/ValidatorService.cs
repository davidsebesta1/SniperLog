using SniperLog.Extensions.CustomXamlComponents;
using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Services
{
    /// <summary>
    /// A service class used for form validation of any kind.
    /// </summary>
    public class ValidatorService
    {
        public const int BaseSize = 16;

        private readonly Dictionary<CustomEntryBase, bool> _validationValues = new Dictionary<CustomEntryBase, bool>(BaseSize);
        private readonly Dictionary<CustomEntryBase, Func<object, bool>> _validationFunctions = new Dictionary<CustomEntryBase, Func<object, bool>>(BaseSize);

        /// <summary>
        /// Gets whether are all fields validated.
        /// </summary>
        public bool AllValid => _validationValues.Values.All(x => x == true);

        /// <summary>
        /// Base constructor.
        /// </summary>
        public ValidatorService()
        {

        }

        /// <summary>
        /// Attempts to add validation. Fails if the entry already exists.
        /// </summary>
        /// <param name="entry">Entry to be validated.</param>
        /// <param name="func">Function to validate the entry with.</param>
        /// <param name="defaultValidation">Default validation value.</param>
        /// <returns>True of false depending whether was the entry added.</returns>
        public bool TryAddValidation(CustomEntryBase entry, Func<object, bool> func, bool defaultValidation = false)
        {
            if (_validationFunctions.ContainsKey(entry))
                return false;

            _validationValues.Add(entry, defaultValidation);
            _validationFunctions.Add(entry, func);

            entry.OnEntryInputChanged += EntryInput;

            return true;
        }

        /// <summary>
        /// Attempts to remove validation. Fails if the entry is not added to the validation.
        /// </summary>
        /// <param name="entry">Entry to be removed from validation.</param>
        /// <returns>Whether was the validation entry removed.</returns>
        public bool TryRemoveValidation(CustomEntryBase entry)
        {
            bool res = _validationValues.Remove(entry) && _validationFunctions.Remove(entry);

            if (res)
                entry.OnEntryInputChanged -= EntryInput;        

            return res;
        }

        /// <summary>
        /// Clears all validation values.
        /// </summary>
        public void ClearAll()
        {
            while (_validationValues.Count > 0)
            {
                CustomEntryBase entry = _validationValues.ElementAt(0).Key;
                TryRemoveValidation(entry);
            }
        }

        /// <summary>
        /// Revalidates all entries.
        /// </summary>
        public void RevalidateAll()
        {
            foreach (var entry in _validationValues)
            {
                switch (entry.Key)
                {
                    case CustomTextEntry txtEntry:
                        EntryInput(txtEntry, txtEntry.TextValue);
                        break;
                    case CustomMultilineTextEntry customMultilineTextEntry:
                        EntryInput(customMultilineTextEntry, customMultilineTextEntry.TextValue);
                        break;
                    case CustomSwitchEntry switchEntry:
                        EntryInput(switchEntry, switchEntry.SelectedOption);
                        break;
                    case CustomImagePickerEntry imagePickerEntry:
                        EntryInput(imagePickerEntry, imagePickerEntry.SelectedImagePath);
                        break;
                    case CustomPickerEntry customPickerEntry:
                        EntryInput(customPickerEntry, customPickerEntry.SelectedItem);
                        break;
                }
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
