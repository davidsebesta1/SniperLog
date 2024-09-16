namespace SniperLog.Extensions
{
    public static class AppDataFileHelper
    {
        public static string AppDataPath
        {
            get
            {
#if WINDOWS
                return Environment.ExpandEnvironmentVariables("%APPDATA%");
#else
                return FileSystem.Current.AppDataDirectory;
#endif
            }
        }

        public static bool IsPathInAppData(string path)
        {
            return path.StartsWith(AppDataPath);
        }

        public static string GetPathFromAppData(string relativePath)
        {
#if WINDOWS
            if (string.IsNullOrEmpty(relativePath))
            {
                return Environment.ExpandEnvironmentVariables("%APPDATA%");
            }
            string pathRel = Path.Combine("%APPDATA%", "SniperLog", relativePath);
            return Environment.ExpandEnvironmentVariables(pathRel);
#else
            if (string.IsNullOrEmpty(relativePath))
            {
                return FileSystem.Current.AppDataDirectory;
            }

            return Path.Combine(FileSystem.Current.AppDataDirectory, relativePath);
#endif
        }

        public static async Task SaveFileToLocationAsync(string relativePath, byte[] data)
        {
            string fullPath = GetPathFromAppData(relativePath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            await File.WriteAllBytesAsync(fullPath, data);
        }

        public static void SaveFileToLocation(string relativePath, byte[] data)
        {
            string fullPath = GetPathFromAppData(relativePath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            File.WriteAllBytes(fullPath, data);
        }

        public static bool DeleteFolderFromLocation(string relativePath)
        {
            if (!Directory.Exists(relativePath))
            {
                return false;
            }

            Directory.Delete(relativePath, true);
            return true;
        }
    }
}