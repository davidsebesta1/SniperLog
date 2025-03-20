using Compunet.YoloSharp.Data;
using Mopups.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SniperLog.Config;
using SniperLog.Extensions;
using SniperLog.Pages.Other;
using SniperLog.Services.AI;
using SniperLog.ViewModels;

namespace SniperLog.Pages
{
    public partial class MainPage : ContentPage
    {
        private BulletHoleDetectionService _detector;
        private string _imagePath;
        private YoloResult<Detection> _results;
        private SKBitmap _bitmap;

        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000);
            VersionControl tracking = ApplicationConfigService.GetConfig<VersionControl>();

            if (tracking.FirstLaunchEver)
            {
                await MopupService.Instance.PushAsync(ServicesHelper.GetService<InitialSetupPopupPage>());
            }

            _detector = new BulletHoleDetectionService(await MauiExtensions.GetRawFilePathAsync("best.onnx"));
            _imagePath = await MauiExtensions.GetRawFilePathAsync("testTarget.jpg");

            _bitmap = SKBitmap.Decode(_imagePath);
            TargetImage.Source = ImageSource.FromFile(_imagePath);
        }

        private async void OnDetectClicked(object sender, EventArgs e)
        {
            if (_detector == null || string.IsNullOrEmpty(_imagePath))
            {
                await DisplayAlert("Error", "Model or image not loaded.", "OK");
                return;
            }

            _results = await _detector.DetectObjects(_imagePath);
            OverlayCanvas.InvalidateSurface();
        }

        private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var canvasWidth = e.Info.Width;
            var canvasHeight = e.Info.Height;

            canvas.Clear(SKColors.White);

            if (_bitmap == null)
                return;

            var imageWidth = _bitmap.Width;
            var imageHeight = _bitmap.Height;

            float scaleX = (float)canvasWidth / imageWidth;
            float scaleY = (float)canvasHeight / imageHeight;
            float scale = Math.Min(scaleX, scaleY);

            float scaledWidth = imageWidth * scale;
            float scaledHeight = imageHeight * scale;
            float xOffset = (canvasWidth - scaledWidth) / 2;
            float yOffset = (canvasHeight - scaledHeight) / 2;

            var destRect = new SKRect(xOffset, yOffset, xOffset + scaledWidth, yOffset + scaledHeight);
            canvas.DrawBitmap(_bitmap, destRect);

            if (_results != null)
            {
                foreach (var detection in _results)
                {
                    float x = xOffset + detection.Bounds.X * scale;
                    float y = yOffset + detection.Bounds.Y * scale;
                    float width = detection.Bounds.Width * scale;
                    float height = detection.Bounds.Height * scale;

                    DrawBoundingBox(canvas, x, y, width, height);
                }
            }
        }

        private void DrawBoundingBox(SKCanvas canvas, float x, float y, float width, float height)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.Red;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 3;
                canvas.DrawRect(x, y, width, height, paint);
            }
        }
    }
}
