using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomSearchBar : ContentView
{
    #region Bindables

    public static readonly BindableProperty TextValueProperty = BindableProperty.Create(nameof(TextValue), typeof(string), typeof(Frame), string.Empty);
    public static readonly BindableProperty PlaceholderTextValueProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(Frame), string.Empty);

    public static readonly BindableProperty EnterCommandProperty = BindableProperty.Create(nameof(EnterCommand), typeof(ICommand), typeof(Entry), null);

    #endregion

    #region Properties

    public string TextValue
    {
        get => (string)GetValue(TextValueProperty);
        set => SetValue(TextValueProperty, value);
    }

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextValueProperty);
        set => SetValue(PlaceholderTextValueProperty, value);
    }

    public ICommand EnterCommand
    {
        get => (ICommand)GetValue(EnterCommandProperty);
        set => SetValue(EnterCommandProperty, value);
    }

    #endregion

    #region Ctro

    public CustomSearchBar()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (EnterCommand != null && EnterCommand.CanExecute(e.NewTextValue))
        {
            EnterCommand.Execute(e.NewTextValue);
        }
    }

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