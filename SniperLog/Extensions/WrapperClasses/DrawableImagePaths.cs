using SkiaSharp;

namespace SniperLog.Extensions.WrapperClasses
{
    public partial class DrawableImagePaths : ObservableObject, IEquatable<DrawableImagePaths?>
    {
        public static readonly string OverDrawPostFix = "_overdraw";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CombinedImageSrc))]
        private string _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CombinedImageSrc))]
        private string _overDrawPath;

        public ImageSource CombinedImageSrc
        {
            get
            {
                //Img doesnt exists, return nothing
                if (!File.Exists(ImagePath))
                {
                    return null;
                }

                //Overdraw doesnt exists, return base image
                if (!File.Exists(OverDrawPath))
                {
                    return ImageSource.FromStream(() => File.OpenRead(ImagePath));
                }


                using SKBitmap baseBitmap = SKBitmap.Decode(ImagePath);
                using SKBitmap overlayBitmap = SKBitmap.Decode(OverDrawPath);

                SKImageInfo imageInfo = new SKImageInfo(baseBitmap.Width, baseBitmap.Height);

                using SKBitmap finalBitmap = new SKBitmap(imageInfo);
                using SKCanvas canvas = new SKCanvas(finalBitmap);

                canvas.DrawBitmap(baseBitmap, 0, 0);
                canvas.DrawBitmap(overlayBitmap, 0, 0);

                using SKImage skImage = SKImage.FromBitmap(finalBitmap);

                MemoryStream outputStream = new MemoryStream();
                using SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100);

                skData.SaveTo(outputStream);
                outputStream.Position = 0;

                return ImageSource.FromStream(() => outputStream);
            }
        }

        public DrawableImagePaths(string imagePath, string overDraw)
        {
            ImagePath = imagePath;
            OverDrawPath = overDraw;
        }

        public DrawableImagePaths(string imagePath)
        {
            ImagePath = imagePath;
            OverDrawPath = ImagePath + OverDrawPostFix;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as DrawableImagePaths);
        }

        public bool Equals(DrawableImagePaths? other)
        {
            return other is not null && ImagePath == other.ImagePath;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ImagePath);
        }

        public static implicit operator string(DrawableImagePaths paths) => paths.ImagePath;
        public static implicit operator DrawableImagePaths(string path) => new DrawableImagePaths(path);

        public static bool operator ==(DrawableImagePaths? left, DrawableImagePaths? right)
        {
            return EqualityComparer<DrawableImagePaths>.Default.Equals(left, right);
        }

        public static bool operator !=(DrawableImagePaths? left, DrawableImagePaths? right)
        {
            return !(left == right);
        }
    }
}
