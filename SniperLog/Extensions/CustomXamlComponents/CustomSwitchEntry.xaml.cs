using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents
{
    /// <summary>
    /// A custom entry that mimics a switch with two options and text for each option.
    /// </summary>
    public partial class CustomSwitchEntry : CustomEntryBase
    {
        #region Bindables

        /// <summary>
        /// Bindable property for the <see cref="SelectedOption"/>.
        /// </summary>
        public static readonly BindableProperty SelectedOptionProperty = BindableProperty.Create(nameof(SelectedOption), typeof(int), typeof(Frame), 1);

        /// <summary>
        /// Bindable property for the <see cref="LeftOptionText"/>.
        /// </summary>
        public static readonly BindableProperty LeftOptionTextProperty = BindableProperty.Create(nameof(LeftOptionText), typeof(string), typeof(Label), string.Empty);

        /// <summary>
        /// Bindable property for the <see cref="RightOptionText"/>.
        /// </summary>
        public static readonly BindableProperty RightOptionTextProperty = BindableProperty.Create(nameof(RightOptionText), typeof(string), typeof(Label), string.Empty);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected option (either 0 or 1).
        /// </summary>
        public int SelectedOption
        {
            get => (int)GetValue(SelectedOptionProperty);
            set
            {
                SetValue(SelectedOptionProperty, value);
                OnEntryInputChanged?.Invoke(this, value);
                EntryInputChangedCommand?.Execute(value);
            }
        }

        /// <summary>
        /// Gets or sets the left option text displayed next to the switch.
        /// </summary>
        public string LeftOptionText
        {
            get => (string)GetValue(LeftOptionTextProperty);
            set => SetValue(LeftOptionTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the right option text displayed next to the switch.
        /// </summary>
        public string RightOptionText
        {
            get => (string)GetValue(RightOptionTextProperty);
            set => SetValue(RightOptionTextProperty, value);
        }

        #endregion

        #region Constructor

        /// <inheritdoc/>
        public CustomSwitchEntry()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Tapped(object sender, TappedEventArgs e)
        {
            SelectedOption = SelectedOption == 1 ? 0 : 1;
        }

        #endregion
    }
}
