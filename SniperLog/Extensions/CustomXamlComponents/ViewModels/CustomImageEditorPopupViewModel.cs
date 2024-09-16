using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics.Platform;
using Mopups.Services;
using SniperLog.Extensions.WrapperClasses;
using SniperLog.ViewModels;
using System.Collections.ObjectModel;
using System.IO;

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
        private async Task EditImage(DrawingView drawingView)
        {
            if (Lines == null || Lines.Count <= 0)
            {
                return;
            }

            if (!File.Exists(BackgroundImage.ImagePath))
            {
                return;
            }

            string tmpSavePath = Path.Combine(FileSystem.Current.CacheDirectory, Guid.NewGuid().ToString() + ".png");

            using Microsoft.Maui.Graphics.IImage originalImage = PlatformImage.FromStream(File.OpenRead(BackgroundImage.ImagePath));

            int targetWidth = (int)originalImage.Width;
            int targetHeight = (int)originalImage.Height;


            using (FileStream localFileStream = File.OpenWrite(tmpSavePath))
            {
                using (Stream stream = drawingView.CaptureDrawingView())
                {
                    using Microsoft.Maui.Graphics.IImage img = PlatformImage.FromStream(stream);
                    using Microsoft.Maui.Graphics.IImage imgResized = img.Resize(targetWidth, targetHeight, ResizeMode.Fit);

                    await imgResized.SaveAsync(localFileStream, ImageFormat.Png);
                }
            }

            BackgroundImage.OverDrawPath = tmpSavePath;
            await MopupService.Instance.PopAsync();
        }
    }
}
