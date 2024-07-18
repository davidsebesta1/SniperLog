
using SniperLog.Extensions.CustomXamlComponents.Abstract;

namespace SniperLog.Extensions.CustomXamlComponents;

public partial class CustomImagePickerEntry : CustomEntryBase
{
    public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int), typeof(Grid), 0, propertyChanged: OnEntryHeightChanged);

    private static readonly BindableProperty EntryHeightFinalProperty = BindableProperty.Create(nameof(EntryHeightFinal), typeof(int), typeof(Grid), BaseHeight);
    private static readonly BindableProperty EntryRowDefsProperty = BindableProperty.Create(nameof(EntryRowDefs), typeof(RowDefinitionCollection), typeof(Grid), null);

    public static readonly BindableProperty SelectedImagePathProperty = BindableProperty.Create(nameof(SelectedImagePath), typeof(string), typeof(Frame), null, propertyChanged: OnSelectedImagePathChanged);
    public static readonly BindableProperty IsImageSelectedProperty = BindableProperty.Create(nameof(IsImageSelected), typeof(bool), typeof(Frame), false);
    private static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(nameof(StrokeThickness), typeof(int), typeof(Border), 3);

    public const int BaseHeight = 205;
    public const int FirstRowBaseHeight = 35;
    public const int EntryRowBaseHeight = 170;

    public int EntryHeight
    {
        get
        {
            return (int)GetValue(EntryHeightProperty);
        }
        set
        {
            SetValue(EntryHeightProperty, value);
        }
    }

    private static void OnEntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomImagePickerEntry customTextEntry)
        {
            customTextEntry.EntryHeightFinal = (int)newValue + BaseHeight + 35;
            customTextEntry.EntryRowDefs = [new RowDefinition() { Height = FirstRowBaseHeight }, new RowDefinition() { Height = EntryRowBaseHeight + (int)newValue }];

            customTextEntry.OnPropertyChanged(nameof(EntryHeightFinal));
            customTextEntry.OnPropertyChanged(nameof(EntryRowDefs));
        }
    }

    public int EntryHeightFinal
    {
        get
        {
            return (int)GetValue(EntryHeightFinalProperty);
        }
        set
        {
            SetValue(EntryHeightFinalProperty, value);
        }
    }

    public RowDefinitionCollection EntryRowDefs
    {
        get
        {
            return (RowDefinitionCollection)GetValue(EntryRowDefsProperty);
        }
        set
        {
            SetValue(EntryRowDefsProperty, value);
        }
    }

    public string SelectedImagePath
    {
        get
        {
            return (string)GetValue(SelectedImagePathProperty);
        }
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
            customTextEntry.IsImageSelected = !string.IsNullOrEmpty((string)newValue);
            customTextEntry.OnPropertyChanged(nameof(IsImageSelected));

            customTextEntry.StrokeThickness = customTextEntry.IsImageSelected ? 0 : 3;
            customTextEntry.OnPropertyChanged(nameof(StrokeThickness));
        }
    }

    public bool IsImageSelected
    {
        get
        {
            return (bool)GetValue(IsImageSelectedProperty);
        }
        private set
        {
            SetValue(IsImageSelectedProperty, value);
        }
    }

    private int StrokeThickness
    {
        get
        {
            return (int)GetValue(StrokeThicknessProperty);
        }
        set
        {
            SetValue(StrokeThicknessProperty, value);
        }
    }

    public CustomImagePickerEntry()
    {
        InitializeComponent();

        OnPropertyChanged(nameof(EntryHeightFinal));
        OnPropertyChanged(nameof(EntryRowDefs));
    }

    private async void GalleryOption_Tapped(object sender, TappedEventArgs e)
    {
        FileResult? result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions()
        {
            Title = "Background image"
        });
        if (result != null)
        {
            SelectedImagePath = result.FullPath;
        }
    }

    private void DeleteImage_Tapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedImagePath))
        {
            return;
        }

        if (Path.GetDirectoryName(SelectedImagePath) == Path.GetDirectoryName(FileSystem.CacheDirectory))
        {
            File.Delete(SelectedImagePath);
        }

        SelectedImagePath = string.Empty;
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

                SelectedImagePath = localFilePath;
            }
        }
        catch (IOException ioException)
        {
            await Shell.Current.DisplayAlert("Error at manipulating with files", ioException.Message, "Okay");
        }
    }
}