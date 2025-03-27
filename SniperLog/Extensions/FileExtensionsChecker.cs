
namespace SniperLog.Extensions;

/// <summary>
/// Static extension class for checking file type.
/// </summary>
public static class FileExtensionsChecker
{
    /// <summary>
    /// Common image file extensions.
    /// </summary>
    public static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".jpe", ".bmp", ".png"];

    /// <summary>
    /// Checks whether provided file on this path is a image type.
    /// </summary>
    /// <param name="fullPath">Absolute path to the file.</param>
    /// <returns>Whether the file is a common image file.</returns>
    public static bool IsImageFile(string fullPath)
    {
        if (!File.Exists(fullPath))
            return false;

        if (!Path.HasExtension(fullPath))
            return false;

        string extension = Path.GetExtension(fullPath).ToLower();
        return ImageExtensions.Any(n => n == extension);
    }
}

