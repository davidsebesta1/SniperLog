using System.Windows.Input;

namespace SniperLog.Extensions.CustomXamlComponents.Abstract;

/// <summary>
/// Base class for custom entry views providing standard bindable properties and events
/// </summary>
public partial class CustomEntryBase : ContentView
{
    /// <summary>
    /// Command binding for changing input, it is programmers responsibility to invoke this event when something changes
    /// </summary>
    public static readonly BindableProperty EntryInputChangedCommandProperty = BindableProperty.Create(nameof(EntryInputChangedCommand), typeof(ICommand), typeof(Entry), null);

    public static readonly BindableProperty IsMandatoryProperty = BindableProperty.Create(nameof(IsMandatory), typeof(bool), typeof(Label), true);

    public static readonly BindableProperty EntryTitleProperty = BindableProperty.Create(nameof(EntryTitle), typeof(string), typeof(Label), string.Empty);
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(Label), string.Empty);
    public static readonly BindableProperty ErrorTextVisibleProperty = BindableProperty.Create(nameof(ErrorTextVisible), typeof(bool), typeof(Label), false);
    public static readonly BindableProperty EntryTitleSubtextProperty = BindableProperty.Create(nameof(EntryTitleSubtext), typeof(string), typeof(Label), null);

    public ICommand EntryInputChangedCommand
    {
        get
        {
            return (ICommand)GetValue(EntryInputChangedCommandProperty);
        }
        set
        {
            SetValue(EntryInputChangedCommandProperty, value);
        }
    }
    public EventHandler<object> OnEntryInputChanged;

    public bool IsMandatory
    {
        get
        {
            return (bool)GetValue(IsMandatoryProperty);
        }
        set
        {
            SetValue(IsMandatoryProperty, value);
        }
    }

    public string EntryTitle
    {
        get
        {
            return (string)GetValue(EntryTitleProperty);
        }
        set
        {
            SetValue(EntryTitleProperty, value);
        }
    }

    public string EntryTitleSubtext
    {
        get
        {
            return (string)GetValue(EntryTitleSubtextProperty);
        }
        set
        {
            SetValue(EntryTitleSubtextProperty, value);
        }
    }

    public string ErrorText
    {
        get
        {
            return (string)GetValue(ErrorTextProperty);
        }
        set
        {
            SetValue(ErrorTextProperty, value);
        }
    }

    public bool ErrorTextVisible
    {
        get
        {
            return (bool)GetValue(ErrorTextVisibleProperty);
        }
        set
        {
            SetValue(ErrorTextVisibleProperty, value);
        }
    }

    public CustomEntryBase()
    {

    }
}