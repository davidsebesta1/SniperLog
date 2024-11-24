using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using SkiaSharp;

namespace SniperLog.Extensions
{
    /// <summary>
    /// A static extension class for extending <see cref="DrawingView"/>,
    /// </summary>
    public static class DrawingViewExtensions
    {
        /// <summary>
        /// Captures whole drawing view area and returns a stream that contains it.
        /// <para>Please dispose the stream yourself.</para>
        /// </summary>
        /// <param name="drawingView">This drawing view.</param>
        /// <returns>A new stream.</returns>
        /// <exception cref="InvalidOperationException">Throws if the drawing view has no size.</exception>
        public static Stream CaptureDrawingView(this DrawingView drawingView)
        {
            int width = (int)drawingView.Width;
            int height = (int)drawingView.Height;

            if (width == 0 || height == 0)
            {
                throw new InvalidOperationException("The DrawingView has no size.");
            }

            SKBitmap bitmap = new SKBitmap(width, height);

            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.Transparent);

                foreach (IDrawingLine line in drawingView.Lines)
                {
                    line.LineColor.ToRgb(out byte r, out byte g, out byte b);
                    SKPaint paint = new SKPaint
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = new SKColor(r, g, b),
                        StrokeWidth = (float)line.LineWidth,
                        IsAntialias = true
                    };

                    SKPoint[] points = line.Points.Select(p => new SKPoint((float)p.X, (float)p.Y)).ToArray();

                    if (points.Length > 1)
                    {
                        canvas.DrawPoints(SKPointMode.Polygon, points, paint);
                    }
                }
            }

            using (SKImage image = SKImage.FromBitmap(bitmap))
            {
                SKData encodedImage = image.Encode(SKEncodedImageFormat.Png, 100);
                MemoryStream stream = new MemoryStream();
                encodedImage.SaveTo(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
    }
}
