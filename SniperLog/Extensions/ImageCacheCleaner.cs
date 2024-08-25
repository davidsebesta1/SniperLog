
namespace SniperLog.Extensions
{
    public static class ImageCacheCleaner
    {
        public static void ClearCache()
        {
            string imageManagerDiskCache = Path.Combine(FileSystem.CacheDirectory);

            foreach (string dir in Directory.GetDirectories(imageManagerDiskCache))
            {
                TraverseDirectoryAndDeleteImgFiles(dir);
            }
        }

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
}