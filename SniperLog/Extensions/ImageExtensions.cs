using SkiaSharp;

namespace SniperLog.Extensions;

/// <summary>
/// Static helpers and extensions for images.
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    /// Amount of channels in RGB space.
    /// </summary>
    public const int RGBChannels = 3;

    /// <summary>
    /// Converts the image to usable float[] array of normalized RGB values.
    /// </summary>
    /// <param name="imagePath">Target absolute image path.</param>
    /// <param name="targetWidth">Target width to downscale/upscale to.</param>
    /// <param name="targetHeight">Targwt height to downscale/upscale to.</param>
    /// <returns>Float array of RGB values.</returns>
    /// <exception cref="Exception">If the image couldn't be resized.</exception>
    public static float[] ProcessImage(string imagePath, int targetWidth = 640, int targetHeight = 640)
    {
        using FileStream inputStream = File.OpenRead(imagePath);
        using SKBitmap bitmap = SKBitmap.Decode(inputStream);

        using SKBitmap resizedBitmap = bitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.High);

        if (resizedBitmap == null)
            throw new Exception("Failed to resize image.");

        float[] floatData = new float[targetWidth * targetHeight * RGBChannels];

        // Normalize
        int index = 0;
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                SKColor pixel = resizedBitmap.GetPixel(x, y);

                floatData[index++] = pixel.Red / 255.0f;
                floatData[index++] = pixel.Green / 255.0f;
                floatData[index++] = pixel.Blue / 255.0f;
            }
        }

        return floatData;
    }
}

