using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using SniperLog.Extensions;
using SniperLog.Services.Serialization;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Other;

public partial class ImportExportPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<Firearm> _firearms;

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

    public ImportExportPageViewModel(DataCacherService<Firearm> firearmService) : base()
    {
        PageTitle = "Import/Export";
        _firearmsService = firearmService;
    }

    [RelayCommand]
    private async Task LoadFirearms()
    {
        Firearms = await _firearmsService.GetAll();
    }

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

