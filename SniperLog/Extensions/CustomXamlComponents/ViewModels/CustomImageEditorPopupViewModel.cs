using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Compunet.YoloSharp.Data;
using Microsoft.Maui.Graphics.Platform;
using Mopups.Services;
using SixLabors.ImageSharp;
using SkiaSharp;
using SniperLog.Extensions.WrapperClasses;
using SniperLog.Services.AI;
using SniperLog.ViewModels;
using System.Collections.ObjectModel;
using Color = Microsoft.Maui.Graphics.Color;
using PointF = Microsoft.Maui.Graphics.PointF;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels;

/// <summary>
/// <see cref="CustomImageEditorPopup"/>'s viewmodel.
/// </summary>
public partial class CustomImageEditorPopupViewModel : BaseViewModel
{
    /// <summary>
    /// Paths to the image and editable image.
    /// </summary>
    [ObservableProperty]
    private DrawableImagePaths _backgroundImage;

    /// <summary>
    /// Drawn lines.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<IDrawingLine> _lines;

    /// <summary>
    /// Whether the <see cref="_bHoleDetector"/> is running.
    /// </summary>
    [ObservableProperty]
    private bool _bulletHoleDetectorRunning;

    /// <summary>
    /// Parent picker entry.
    /// </summary>
    public CustomImagePickerEntry Entry;

    private BulletHoleDetectionService _bHoleDetector;

    private CancellationTokenSource _detectorCancelToken;

    /// <inheritdoc/>
    public CustomImageEditorPopupViewModel(BulletHoleDetectionService service) : base()
    {
        PageTitle = "Image Editor";
        _bHoleDetector = service;
    }

    [RelayCommand]
    private async Task Cancel()
    {
        if (_detectorCancelToken != null)
            await _detectorCancelToken.CancelAsync();

        await MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    private async Task EditImage(DrawingView drawingView)
    {
        if (Lines == null || Lines.Count <= 0)
            return;

        if (!File.Exists(BackgroundImage.ImagePath))
            return;

        string tmpSavePath = Path.Combine(FileSystem.Current.CacheDirectory, Guid.NewGuid().ToString() + ".png");
        using (Microsoft.Maui.Graphics.IImage originalImage = PlatformImage.FromStream(File.OpenRead(BackgroundImage.ImagePath)))
        {
            int targetWidth = (int)originalImage.Width;
            int targetHeight = (int)originalImage.Height;

            using (Stream stream = drawingView.CaptureDrawingView())
            using (SKBitmap drawingBitmap = SKBitmap.Decode(stream))
            {
                using (SKBitmap resizedBitmap = drawingBitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.High))
                {
                    using (FileStream localFileStream = File.OpenWrite(tmpSavePath))
                    using (SKImage img = SKImage.FromBitmap(resizedBitmap))
                    using (SKData data = img.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        data.SaveTo(localFileStream);
                    }
                }
            }
        }

        BackgroundImage.OverDrawPath = tmpSavePath;
        await MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    private async Task SearchHoles(DrawingView drawingView)
    {
        if (!File.Exists(BackgroundImage.ImagePath) || drawingView == null)
            return;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        _detectorCancelToken = tokenSource;

        BulletHoleDetectorRunning = true;
        Task task = Task.Run(async () =>
        {
            YoloResult<Detection> result = await _bHoleDetector.DetectObjects(BackgroundImage.ImagePath);

            if (result == null || result.Count == 0)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Lines == null)
                    Lines = new ObservableCollection<IDrawingLine>();

                using Microsoft.Maui.Graphics.IImage originalImage = PlatformImage.FromStream(File.OpenRead(BackgroundImage.ImagePath));

                float imageWidth = originalImage.Width;
                float imageHeight = originalImage.Height;

                float drawingWidth = (float)drawingView.Width;
                float drawingHeight = (float)drawingView.Height;

                // Scale multiplier from map image coordinates to drawing view coordinates
                float scaleX = drawingWidth / imageWidth;
                float scaleY = drawingHeight / imageHeight;

                foreach (Detection detection in result)
                {
                    Rectangle bounds = detection.Bounds;

                    // Scale bounding box to match the drawing view size
                    float x = bounds.X * scaleX;
                    float y = bounds.Y * scaleY;
                    float width = bounds.Width * scaleX;
                    float height = bounds.Height * scaleY;

                    Color lineColor = Colors.Red;
                    int lineWidth = 2;

                    AddRectangleToLines(x, y, width, height, lineWidth, lineColor);
                }

                BulletHoleDetectorRunning = false;
            });
        }, tokenSource.Token);

        await task;
    }

    private void AddRectangleToLines(float x, float y, float width, float height, int lineWidth, Color lineColor)
    {
        Lines.Add(new DrawingLine
        {
            Points = new ObservableCollection<PointF> // Top line
            {
                new PointF(x, y),
                new PointF(x + width, y)
            },
            LineColor = lineColor,
            LineWidth = lineWidth
        });

        Lines.Add(new DrawingLine
        {
            Points = new ObservableCollection<PointF> // Right line
            {
                new PointF(x + width, y),
                new PointF(x + width, y + height)
            },
            LineColor = lineColor,
            LineWidth = lineWidth
        });

        Lines.Add(new DrawingLine
        {
            Points = new ObservableCollection<PointF> // Bottom line
            {
                new PointF(x + width, y + height),
                new PointF(x, y + height)
            },
            LineColor = lineColor,
            LineWidth = lineWidth
        });

        Lines.Add(new DrawingLine
        {
            Points = new ObservableCollection<PointF> // left line
            {
                new PointF(x, y + height),
                new PointF(x, y)
            },
            LineColor = lineColor,
            LineWidth = lineWidth
        });
    }
}

