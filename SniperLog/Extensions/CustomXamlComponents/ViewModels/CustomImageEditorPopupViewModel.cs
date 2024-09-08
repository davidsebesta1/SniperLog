using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLog.Extensions.WrapperClasses;
using SniperLog.ViewModels;
using System.Collections.ObjectModel;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels
{
    public partial class CustomImageEditorPopupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private DrawableImagePaths _backgroundImage;

        [ObservableProperty]
        private ObservableCollection<IDrawingLine> _lines;

        public CustomImagePickerEntry Entry;

        public CustomImageEditorPopupViewModel() : base()
        {
            PageTitle = "Image Editor";
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await MopupService.Instance.PopAsync();
        }

        [RelayCommand]
        private async Task EditImage()
        {
            if (Lines == null || Lines.Count <= 0)
            {
                return;
            }

            
        }
    }
}
