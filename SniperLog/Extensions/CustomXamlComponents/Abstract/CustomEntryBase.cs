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

    /// <summary>
    /// Binding for <see cref="IsMandatory"/>.
    /// </summary>
    public static readonly BindableProperty IsMandatoryProperty = BindableProperty.Create(nameof(IsMandatory), typeof(bool), typeof(Label), true);

    /// <summary>
    /// Binding for <see cref="EntryTitle"/>.
    /// </summary>
    public static readonly BindableProperty EntryTitleProperty = BindableProperty.Create(nameof(EntryTitle), typeof(string), typeof(Label), string.Empty);

    /// <summary>
    /// Binding for <see cref="ErrorText"/>.
    /// </summary>
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(Label), string.Empty);

    /// <summary>
    /// Binding for <see cref="ErrorTextVisible"/>.
    /// </summary>
    public static readonly BindableProperty ErrorTextVisibleProperty = BindableProperty.Create(nameof(ErrorTextVisible), typeof(bool), typeof(Label), false);

    /// <summary>
    /// Binding for <see cref="EntryTitle"/>.
    /// </summary>
    public static readonly BindableProperty EntryTitleSubtextProperty = BindableProperty.Create(nameof(EntryTitleSubtext), typeof(string), typeof(Label), null);

    /// <summary>
    /// Event for when entry input has been changed.
    /// </summary>
    public EventHandler<object> OnEntryInputChanged;

    /// <summary>
    /// Entry input changed command.
    /// </summary>
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

    /// <summary>
    /// Gets or sets whether the field is mandatory.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the entry title text.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the entry title subtext.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the entry title error text.
    /// </summary>
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

    /// <summary>
    /// Gets or sets whether the <see cref="ErrorText"/> is visible.
    /// </summary>
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

    /// <summary>
    /// Base ctor does not initialize components itself
    /// </summary>
    public CustomEntryBase()
    {
        
    }
}