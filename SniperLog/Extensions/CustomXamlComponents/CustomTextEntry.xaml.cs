using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents
{
    /// <summary>
    /// A custom entry component that provides customizable text input functionality.
    /// </summary>
    public partial class CustomTextEntry : CustomEntryBase
    {
        #region Bindables

        /// <summary>
        /// Bindable property for the <see cref="TextValue"/> entered in the custom entry.
        /// </summary>
        public static readonly BindableProperty TextValueProperty =
            BindableProperty.Create(nameof(TextValue), typeof(string), typeof(Entry), string.Empty);

        /// <summary>
        /// Bindable property for the <see cref="PlaceholderText"/> displayed in the custom entry.
        /// </summary>
        public static readonly BindableProperty PlaceholderTextValueProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(Entry), string.Empty);

        /// <summary>
        /// Bindable property for the <see cref="Keyboard"/> used by the custom entry.
        /// </summary>
        public static readonly BindableProperty KeyboardValueProperty =
            BindableProperty.CreateAttached(nameof(Keyboard), typeof(Keyboard), typeof(CustomEntryBase), Keyboard.Default);

        /// <summary>
        /// Bindable property for the <see cref="EntryHeight"/>.
        /// </summary>
        public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

        private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);

        private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

        #endregion

        #region Constants

        /// <summary>
        /// The base height for the entry.
        /// </summary>
        public const int BaseHeight = 100;

        /// <summary>
        /// The base height for the first row in the entry.
        /// </summary>
        public const int FirstRowBaseHeight = 30;

        /// <summary>
        /// The base height for the entry row.
        /// </summary>
        public const int EntryRowBaseHeight = 55;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTextEntry"/> class.
        /// </summary>
        public CustomTextEntry()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the keyboard type for the custom entry.
        /// </summary>
        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardValueProperty);
            set => SetValue(KeyboardValueProperty, value);
        }

        /// <summary>
        /// Gets or sets the height of the entry.
        /// </summary>
        public int EntryHeight
        {
            get => (int)GetValue(EntryHeightProperty);
            set => SetValue(EntryHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the final height of the entry after adjustments.
        /// </summary>
        public int EntryHeightFinal
        {
            get => (int)GetValue(EntryHeightFinalProperty);
            set => SetValue(EntryHeightFinalProperty, value);
        }

        /// <summary>
        /// Gets or sets the row definitions for the entry's layout.
        /// </summary>
        public RowDefinitionCollection EntryRowDefs
        {
            get => (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
            set => SetValue(EntryRowDefsProperty, value);
        }

        /// <summary>
        /// Gets or sets the text value entered in the custom entry.
        /// </summary>
        public string TextValue
        {
            get => (string)GetValue(TextValueProperty);
            set
            {
                SetValue(TextValueProperty, value);
                OnEntryInputChanged?.Invoke(this, value);
                EntryInputChangedCommand?.Execute(value);
            }
        }

        /// <summary>
        /// Gets or sets the placeholder text displayed in the custom entry.
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextValueProperty);
            set => SetValue(PlaceholderTextValueProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the entry height changes. Updates the final height and row definitions.
        /// </summary>
        /// <param name="bindable">The bindable object.</param>
        /// <param name="oldValue">The old value of the entry height.</param>
        /// <param name="newValue">The new value of the entry height.</param>
        private static void OnEntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomTextEntry customTextEntry)
            {
                customTextEntry.EntryHeightFinal = (int)newValue + BaseHeight;
                customTextEntry.EntryRowDefs = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = FirstRowBaseHeight },
                    new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue }
                };
                customTextEntry.OnPropertyChanged(nameof(EntryHeightFinal));
                customTextEntry.OnPropertyChanged(nameof(EntryRowDefs));
            }
        }

        #endregion
    }
}
