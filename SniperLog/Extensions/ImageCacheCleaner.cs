
namespace SniperLog.Extensions;

/// <summary>
/// Static class for clearing image cache. Primary used for android.
/// </summary>
public static class ImageCacheCleaner
{
    /// <summary>
    /// Recursively traverse and delete all image files
    /// </summary>
    /// <param name="dir"></param>
    private static void TraverseDirectoryAndDeleteImgFiles(string dir)
    {
        foreach (string file in Directory.GetFiles(dir))
        {
            if (FileExtensionsChecker.IsImageFile(file))
            {
                File.Delete(file);
            }
        }

        foreach (string subdir in Directory.GetDirectories(dir))
        {
            TraverseDirectoryAndDeleteImgFiles(subdir);
        }
    }
}
