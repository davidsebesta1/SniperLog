namespace SniperLog.Extensions
{
    public static class AppDataFileHelper
    {
        public static string GetPathFromAppData(string relativePath)
        {
#if WINDOWS
            string pathRel = Path.Combine("%APPDATA%", "SniperLog", relativePath);
            return Environment.ExpandEnvironmentVariables(pathRel);
#else
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