using SkiaSharp;

namespace SniperLog.Extensions.WrapperClasses;

/// <summary>
/// Wrapping class for 2 images for <see cref="ShootingRecord"/> and its saved image + overdrawn image over it.
/// </summary>
public partial class DrawableImagePaths : ObservableObject, IEquatable<DrawableImagePaths?>
{
    /// <summary>
    /// Postfix for the image path.
    /// </summary>
    public static readonly string OverDrawPostFix = "_overdraw";

    /// <summary>
    /// Absolute image path to the original image path.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CombinedImageSrc))]
    private string _imagePath;

    /// <summary>
    /// Absolute path to the overdraw for the image.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CombinedImageSrc))]
    private string _overDrawPath;

    private string _tmpFilePath;

    /// <summary>
    /// Combined image from <see cref="ImagePath"/> and <see cref="OverDrawPath"/>
    /// </summary>
    public ImageSource CombinedImageSrc
    {
        get
        {
            if (!File.Exists(ImagePath))
                return null;

            if (!File.Exists(OverDrawPath))
                return ImageSource.FromFile(ImagePath);

            try
            {
                // If the combined image data has already been generated, return it as a new stream
                if (!string.IsNullOrEmpty(_tmpFilePath) && File.Exists(_tmpFilePath))
                    return ImageSource.FromFile(_tmpFilePath);

                using SKBitmap bitmap1 = SKBitmap.Decode(ImagePath);
                using SKBitmap bitmap2 = SKBitmap.Decode(OverDrawPath);

                if (bitmap1 == null || bitmap2 == null)
                    return ImageSource.FromFile(ImagePath);

                int width = Math.Max(bitmap1.Width, bitmap2.Width);
                int height = Math.Max(bitmap1.Height, bitmap2.Height);

                using SKBitmap resultBitmap = new SKBitmap(width, height);
                using SKCanvas canvas = new SKCanvas(resultBitmap);

                canvas.Clear(SKColors.Transparent);

                // Draw the first image
                canvas.DrawBitmap(bitmap1, SKPoint.Empty);

                // Scaling the second image
                SKPaint paint = new SKPaint { FilterQuality = SKFilterQuality.High };
                float scaleX = (float)bitmap1.Width / bitmap2.Width;
                float scaleY = (float)bitmap1.Height / bitmap2.Height;

                SKMatrix scaleMatrix = SKMatrix.CreateScale(scaleX, scaleY);
                canvas.SetMatrix(scaleMatrix);

                // Draw the second image
                canvas.DrawBitmap(bitmap2, SKPoint.Empty, paint);
                canvas.Flush();

                // Encode the result to PNG format and save to byte array
                using SKImage image = SKImage.FromBitmap(resultBitmap);
                using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

                // Return the new ImageSource from the byte array
                _tmpFilePath = Path.Combine(FileSystem.Current.CacheDirectory, Guid.NewGuid().ToString() + ".png");

                using (Stream dataStream = data.AsStream(true))
                {
                    using (FileStream fStream = File.OpenWrite(_tmpFilePath))
                    {
                        dataStream.CopyTo(fStream);
                    }
                }

                return ImageSource.FromFile(_tmpFilePath);
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", ex.ToString(), "OK");
            }

            return null;
        }
    }

    /// <summary>
    /// Ctor with both paths to the images.
    /// </summary>
    /// <param name="imagePath">Original image.</param>
    /// <param name="overDraw">Overdraw image.</param>
    public DrawableImagePaths(string imagePath, string overDraw)
    {
        ImagePath = imagePath;
        OverDrawPath = overDraw;
    }

    /// <summary>
    /// Ctor with path only to the original image. Overdraw is empty.
    /// </summary>
    /// <param name="imagePath">Original image.</param>
    public DrawableImagePaths(string imagePath)
    {
        ImagePath = imagePath;

        if (!string.IsNullOrEmpty(imagePath) && ImagePath.Length > 5)
        {
            OverDrawPath = ImagePath.Insert(ImagePath.Length - 4, OverDrawPostFix);
        }
        else
        {
            OverDrawPath = string.Empty;
        }
    }

    /// <summary>
    /// Ctor from <see cref="ShootingRecordImage"/>.
    /// </summary>
    /// <param name="img">Record image object.</param>
    public DrawableImagePaths(ShootingRecordImage img) : this(img.BackgroundImgPathFull)
    {

    }

    partial void OnImagePathChanged(string? oldValue, string newValue)
    {
        _tmpFilePath = string.Empty;
    }

    partial void OnOverDrawPathChanged(string? oldValue, string newValue)
    {
        _tmpFilePath = string.Empty;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as DrawableImagePaths);
    }

    /// <inheritdoc/>
    public bool Equals(DrawableImagePaths? other)
    {
        return other is not null && ImagePath == other.ImagePath;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ImagePath);
    }

    /// <inheritdoc/>
    public static implicit operator string(DrawableImagePaths paths) => paths.ImagePath;

    /// <inheritdoc/>
    public static implicit operator DrawableImagePaths(string path) => new DrawableImagePaths(path);

    /// <inheritdoc/>
    public static bool operator ==(DrawableImagePaths? left, DrawableImagePaths? right)
    {
        return EqualityComparer<DrawableImagePaths>.Default.Equals(left, right);
    }

    /// <inheritdoc/>
    public static bool operator !=(DrawableImagePaths? left, DrawableImagePaths? right)
    {
        return !(left == right);
    }
}

