using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents
{
    /// <summary>
    /// A custom search bar control with a text entry field and a button to clear the text.
    /// </summary>
    public partial class CustomSearchBar : ContentView
    {
        #region Bindables

        /// <summary>
        /// Bindable property for the entered <see cref="TextValue"/>.
        /// </summary>
        public static readonly BindableProperty TextValueProperty = BindableProperty.Create(nameof(TextValue), typeof(string), typeof(Frame), string.Empty);

        /// <summary>
        /// Bindable property for the <see cref="PlaceholderText"/> of the search bar.
        /// </summary>
        public static readonly BindableProperty PlaceholderTextValueProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(Frame), string.Empty);

        /// <summary>
        /// Bindable property for the <see cref="EnterCommand"/> to execute when the Enter key is pressed.
        /// </summary>
        public static readonly BindableProperty EnterCommandProperty = BindableProperty.Create(nameof(EnterCommand), typeof(ICommand), typeof(Entry), null);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text entered into the search bar.
        /// </summary>
        public string TextValue
        {
            get => (string)GetValue(TextValueProperty);
            set => SetValue(TextValueProperty, value);
        }

        /// <summary>
        /// Gets or sets the placeholder text for the search bar.
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextValueProperty);
            set => SetValue(PlaceholderTextValueProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when the Enter key is pressed.
        /// </summary>
        public ICommand EnterCommand
        {
            get => (ICommand)GetValue(EnterCommandProperty);
            set => SetValue(EnterCommandProperty, value);
        }

        #endregion

        #region Constructor

        /// <inheritdoc/>
        public CustomSearchBar()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the text change event of the entry field. Executes the <see cref="EnterCommand"/> if it can be executed.
        /// </summary>
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EnterCommand != null && EnterCommand.CanExecute(e.NewTextValue))
            {
                EnterCommand.Execute(e.NewTextValue);
            }
        }

        /// <summary>
        /// Handles the image button press event to clear the text value.
        /// Executes the <see cref="EnterCommand"/> with an empty string if it can be executed.
        /// </summary>
        private void ImageButton_Pressed(object sender, EventArgs e)
        {
            TextValue = string.Empty;
            if (EnterCommand != null && EnterCommand.CanExecute(string.Empty))
            {
                EnterCommand.Execute(string.Empty);
            }
        }

        #endregion
    }
}
