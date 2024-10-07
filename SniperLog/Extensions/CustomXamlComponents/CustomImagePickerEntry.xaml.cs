using CommunityToolkit.Maui.Core;
using Mopups.Services;
using SniperLog.Extensions.CustomXamlComponents.Abstract;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;
using SniperLog.Extensions.WrapperClasses;
using System.Collections.ObjectModel;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomImagePickerEntry : CustomEntryBase
{
    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    public static readonly BindableProperty SelectedImagePathProperty = BindableProperty.Create(nameof(SelectedImagePath), typeof(DrawableImagePaths), typeof(Frame), new DrawableImagePaths(string.Empty, string.Empty), propertyChanged: OnSelectedImagePathChanged);
    public static readonly BindableProperty IsImageSelectedProperty = BindableProperty.Create(nameof(IsImageSelected), typeof(bool), typeof(Frame), false);
    public static readonly BindableProperty AllowImageEditingProperty = BindableProperty.Create(nameof(AllowImageEditing), typeof(bool), typeof(Frame), false);
    private static readonly BindableProperty IsImageEditorVisibleProperty = BindableProperty.Create(nameof(IsImageEditorVisible), typeof(bool), typeof(Frame), false);
    private static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(nameof(StrokeThickness), typeof(int), typeof(Border), 3);

    public const int BaseHeight = 205;
    public const int FirstRowBaseHeight = 35;
    public const int EntryRowBaseHeight = 170;

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

    public int EntryHeightFinal
    {
        get => (int)GetValue(EntryHeightFinalProperty);
        set => SetValue(EntryHeightFinalProperty, value);
    }

    public RowDefinitionCollection EntryRowDefs
    {
        get => (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
        set => SetValue(EntryRowDefsProperty, value);
    }

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
            DrawableImagePaths imgPaths;
            if (newValue is string str)
            {
                imgPaths = new DrawableImagePaths(str);
            }
            else if (newValue is DrawableImagePaths paths)
            {
                imgPaths = paths;
            }
            else
            {
                return;
            }

            customTextEntry.IsImageSelected = !string.IsNullOrEmpty(imgPaths.ImagePath);
            customTextEntry.OnPropertyChanged(nameof(IsImageSelected));

            customTextEntry.StrokeThickness = customTextEntry.IsImageSelected ? 0 : 3;
            customTextEntry.OnPropertyChanged(nameof(StrokeThickness));
        }
    }

    public bool IsImageSelected
    {
        get => (bool)GetValue(IsImageSelectedProperty);
        private set
        {
            SetValue(IsImageSelectedProperty, value);
            IsImageEditorVisible = value && AllowImageEditing;
        }
    }

    public bool AllowImageEditing
    {
        get => (bool)GetValue(AllowImageEditingProperty);
        set
        {
            SetValue(AllowImageEditingProperty, value);
            IsImageEditorVisible = value && IsImageSelected;
        }
    }

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
        FileResult? result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions()
        {
            Title = "Background image"
        });
        if (result != null)
        {
            SelectedImagePath.ImagePath = result.FullPath;
            OnPropertyChanged(nameof(SelectedImagePath));
        }
    }

    private void DeleteImage_Tapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedImagePath.ImagePath))
        {
            return;
        }

        if (Path.GetDirectoryName(SelectedImagePath.ImagePath) == Path.GetDirectoryName(FileSystem.CacheDirectory))
        {
            File.Delete(SelectedImagePath.ImagePath);
        }

        SelectedImagePath.ImagePath = string.Empty;
        OnPropertyChanged(nameof(SelectedImagePath));
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

                using (Stream sourceStream = await photo.OpenReadAsync())
                {
                    using (FileStream localFileStream = File.OpenWrite(localFilePath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }
                }

                SelectedImagePath.ImagePath = localFilePath;
                OnPropertyChanged(nameof(SelectedImagePath));
            }
        }
        catch (IOException ioException)
        {
            await Shell.Current.DisplayAlert("Error at manipulating with files", ioException.Message, "Okay");
        }
    }

    private async void EditImage_Tapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedImagePath.ImagePath))
        {
            return;
        }

        CustomImageEditorPopupViewModel vm = (_editorPopup.BindingContext as CustomImageEditorPopupViewModel);
        vm.BackgroundImage = SelectedImagePath;

        if (vm.Lines == null)
        {
            vm.Lines = new ObservableCollection<IDrawingLine>();
        }
        else
        {
            vm.Lines.Clear();
        }

        await MopupService.Instance.PushAsync(_editorPopup);
    }
}