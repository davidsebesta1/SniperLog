using CommunityToolkit.Maui.Core;
using Mopups.Services;
using SniperLog.Extensions.CustomXamlComponents.Abstract;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using SniperLog.Extensions.WrapperClasses;
using System.Collections.ObjectModel;

namespace SniperLog.Extensions.CustomXamlComponents;

/// <summary>
/// A custom entry component for selecting and editing images.
/// </summary>
public partial class CustomImagePickerEntry : CustomEntryBase
{
    /// <summary>
    /// Bindable property for <see cref="EntryHeight"/>.
    /// </summary>
    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    /// <summary>
    /// Bindable property for <see cref="SelectedImagePath"/>.
    /// </summary>
    public static readonly BindableProperty SelectedImagePathProperty = BindableProperty.Create(nameof(SelectedImagePath), typeof(DrawableImagePaths), typeof(Frame), new DrawableImagePaths(string.Empty, string.Empty), propertyChanged: OnSelectedImagePathChanged);

    /// <summary>
    /// Bindable property indicating whether an image is selected.
    /// </summary>
    public static readonly BindableProperty IsImageSelectedProperty = BindableProperty.Create(nameof(IsImageSelected), typeof(bool), typeof(Frame), false);

    /// <summary>
    /// Bindable property indicating whether image editing is allowed.
    /// </summary>
    public static readonly BindableProperty AllowImageEditingProperty = BindableProperty.Create(nameof(AllowImageEditing), typeof(bool), typeof(Frame), false);

    private static readonly BindableProperty IsImageEditorVisibleProperty = BindableProperty.Create(nameof(IsImageEditorVisible), typeof(bool), typeof(Frame), false);
    private static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(nameof(StrokeThickness), typeof(int), typeof(Border), 3);

    /// <summary>
    /// The base height of the entry.
    /// </summary>
    public const int BaseHeight = 205;

    /// <summary>
    /// The height of the first row in the entry.
    /// </summary>
    public const int FirstRowBaseHeight = 35;

    /// <summary>
    /// The height of the entry row.
    /// </summary>
    public const int EntryRowBaseHeight = 170;

    /// <summary>
    /// Gets or sets the height of the entry.
    /// </summary>
    public int EntryHeight
    {
        get => (int)GetValue(EntryHeightProperty);
        set => SetValue(EntryHeightProperty, value);
    }

    private static void OnEntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomImagePickerEntry customTextEntry)
        {
            customTextEntry.EntryHeightFinal = (int)newValue + BaseHeight + FirstRowBaseHeight;
            customTextEntry.EntryRowDefs = [new RowDefinition() { Height = FirstRowBaseHeight }, new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue }];

            customTextEntry.OnPropertyChanged(nameof(EntryHeightFinal));
            customTextEntry.OnPropertyChanged(nameof(EntryRowDefs));
        }
    }

    /// <summary>
    /// Gets or sets the final calculated height of the entry.
    /// </summary>
    public int EntryHeightFinal
    {
        get => (int)GetValue(EntryHeightFinalProperty);
        set => SetValue(EntryHeightFinalProperty, value);
    }

    /// <summary>
    /// Gets or sets the row definitions for the entry.
    /// </summary>
    public RowDefinitionCollection EntryRowDefs
    {
        get => (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
        set => SetValue(EntryRowDefsProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected image path.
    /// </summary>
    public DrawableImagePaths SelectedImagePath
    {
        get => (DrawableImagePaths)GetValue(SelectedImagePathProperty);
        set
        {
            SetValue(SelectedImagePathProperty, value);
            OnEntryInputChanged?.Invoke(this, value);
            EntryInputChangedCommand?.Execute(value);
        }
    }

    private static void OnSelectedImagePathChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomImagePickerEntry customTextEntry)
        {
            DrawableImagePaths imgPaths = newValue switch
            {
                string str => new DrawableImagePaths(str),
                DrawableImagePaths paths => paths,
                _ => null
            };

            if (imgPaths == null) 
                return;

            customTextEntry.IsImageSelected = !string.IsNullOrEmpty(imgPaths.ImagePath);
            customTextEntry.OnPropertyChanged(nameof(IsImageSelected));

            customTextEntry.StrokeThickness = customTextEntry.IsImageSelected ? 0 : 3;
            customTextEntry.OnPropertyChanged(nameof(StrokeThickness));
        }
    }

    /// <summary>
    /// Gets a value indicating whether an image is selected.
    /// </summary>
    public bool IsImageSelected
    {
        get => (bool)GetValue(IsImageSelectedProperty);
        private set
        {
            SetValue(IsImageSelectedProperty, value);
            IsImageEditorVisible = value && AllowImageEditing;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether image editing is allowed.
    /// </summary>
    public bool AllowImageEditing
    {
        get => (bool)GetValue(AllowImageEditingProperty);
        set
        {
            SetValue(AllowImageEditingProperty, value);
            IsImageEditorVisible = value && IsImageSelected;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the image editor is visible.
    /// </summary>
    public bool IsImageEditorVisible
    {
        get => (bool)GetValue(IsImageEditorVisibleProperty);
        set => SetValue(IsImageEditorVisibleProperty, value);
    }

    private int StrokeThickness
    {
        get => (int)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    private CustomImageEditorPopup _editorPopup;

    /// <inheritdoc/>
    public CustomImagePickerEntry()
    {
        InitializeComponent();

        OnPropertyChanged(nameof(EntryHeightFinal));
        OnPropertyChanged(nameof(EntryRowDefs));

        CustomImageEditorPopupViewModel vm = ServicesHelper.GetService<CustomImageEditorPopupViewModel>();
        vm.Entry = this;

        _editorPopup = new CustomImageEditorPopup(vm);
    }

    private async void GalleryOption_Tapped(object sender, TappedEventArgs e)
    {
        FileResult? result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions() { Title = "Background image" });
        if (result != null)
            SelectedImagePath = new DrawableImagePaths(result.FullPath);
    }

    private void DeleteImage_Tapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedImagePath.ImagePath)) 
            return;

        if (Path.GetDirectoryName(SelectedImagePath.ImagePath) == Path.GetDirectoryName(FileSystem.CacheDirectory))
            File.Delete(SelectedImagePath.ImagePath);

        SelectedImagePath = new DrawableImagePaths(string.Empty);
    }

    private async void CameraOption_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert("Unsupported", "Taking photos on this device is not supported", "Okay");
                return;
            }

            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await sourceStream.CopyToAsync(localFileStream);

                SelectedImagePath = new DrawableImagePaths(localFilePath);
            }
        }
        catch (IOException ioException)
        {
            await Shell.Current.DisplayAlert("Error at manipulating with files", ioException.Message, "Okay");
        }
    }

    private async void EditImage_Tapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedImagePath.ImagePath)) return;

        CustomImageEditorPopupViewModel vm = (_editorPopup.BindingContext as CustomImageEditorPopupViewModel);
        vm.BackgroundImage = SelectedImagePath;
        vm.Lines ??= new ObservableCollection<IDrawingLine>();
        vm.Lines.Clear();

        await MopupService.Instance.PushAsync(_editorPopup);
    }
}
