
namespace SniperLog.Extensions
{
    public static class FileExtensionsChecker
    {
        public static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".jpe", ".bmp", ".png"];

        public static bool IsImageFile(string fullPath)
        {
            if (!File.Exists(fullPath)) return false;
            if (!Path.HasExtension(fullPath)) return false;

            string extension = Path.GetExtension(fullPath).ToLower();
            return ImageExtensions.Any(n => n == extension);
        }
    }
}
