using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using SniperLog.Services.Serialization;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Other;

/// <summary>
/// Handles the import/export page logic.
/// </summary>
public partial class ImportExportPageViewModel : BaseViewModel
{
    /// <summary>
    /// All exportable firearms.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Firearm> _firearms;

    /// <summary>
    /// Selected firearm from the list.
    /// </summary>
    [ObservableProperty]
    private Firearm _selectedFirearm;

    private readonly DataCacherService<Firearm> _firearmsService;

    private readonly FilePickerFileType _customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { ".json" } },
                    { DevicePlatform.Android, new[] { ".json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.Tizen, new[] { ".json" } },
                    { DevicePlatform.macOS, new[] { ".json" } },
                });

    /// <summary>
    /// Ctor.
    public ImportExportPageViewModel(DataCacherService<Firearm> firearmService) : base()
    {
        PageTitle = "Import/Export";
        _firearmsService = firearmService;
    }

    /// <summary>
    /// Loads all firearms to the <see cref="Firearms"/>.
    /// </summary>
    [RelayCommand]
    private async Task LoadFirearms()
    {
        Firearms = await _firearmsService.GetAll();
    }

    /// <summary>
    /// Saves the <see cref="Firearm"/> to the JSON file.
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveFirearm()
    {
        FolderPickerResult result = await FolderPicker.Default.PickAsync();
        if (result.IsSuccessful)
        {
            await DatabaseExporterService.ExportFirearmToJson(SelectedFirearm.ID, result.Folder.Path + $"firearm-{SelectedFirearm.Name}-export.json");
            await Shell.Current.DisplayAlert("Export", $"Exported {SelectedFirearm.Name}", "Okay");
        }
        else
        {
            await Shell.Current.DisplayAlert("Export", $"Firearm not exported, no folder selected", "Okay");
        }

    }

    /// <summary>
    /// Loads the <see cref="Firearm"/> and its related objects to the DB if they don't exists yet.
    /// </summary>
    [RelayCommand]
    private async Task LoadFirearm()
    {
        FileResult result = await FilePicker.Default.PickAsync(new PickOptions()
        {
            FileTypes = _customFileType
        });

        if (result != null && (result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase)))
        {
            await DatabaseExporterService.ImportFirearmFromJson(result.FullPath);
            await Shell.Current.DisplayAlert("Import", $"Imported selected file!", "Okay");
            return;
        }

        await Shell.Current.DisplayAlert("Import", $"Unable to import, no file selected.", "Okay");
    }
}

