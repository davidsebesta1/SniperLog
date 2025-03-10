namespace SniperLog.Extensions;

/// <summary>
/// Static helper class for managing appdata files for all sort of operating systems.
/// </summary>
public static class AppDataFileHelper
{
    /// <summary>
    /// Gets the absolute path to system's program app data directory.
    /// </summary>
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

    /// <summary>
    /// Gets whether the provided path is in app data directory or subdirectory.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsPathInAppData(string path)
    {
        return path.StartsWith(AppDataPath);
    }

    /// <summary>
    /// Gets absolute path to the app data from relative path.
    /// <para>
    /// Eg /data/images/shootingranges gets converted to C:/user/username/programfiles..../data/images/shootingranges
    /// </para>
    /// </summary>
    /// <param name="relativePath">Relative path.</param>
    /// <returns>An absolute path to the file.</returns>
    public static string GetPathFromAppData(string relativePath)
    {
#if WINDOWS
            if (string.IsNullOrEmpty(relativePath))
                return Environment.ExpandEnvironmentVariables("%APPDATA%");
            
            string pathRel = Path.Combine("%APPDATA%", "SniperLog", relativePath);
            return Environment.ExpandEnvironmentVariables(pathRel);
#else

        if (string.IsNullOrEmpty(relativePath))
            return FileSystem.Current.AppDataDirectory;

        return Path.Combine(FileSystem.Current.AppDataDirectory, relativePath);
#endif
    }

    /// <summary>
    /// Saves bytes to the location asynchronously. Creates any required directories to the file.
    /// </summary>
    /// <param name="relativePath">Relative path to the app data directory.</param>
    /// <param name="data">Data to save.</param>
    public static async Task SaveFileToLocationAsync(string relativePath, byte[] data)
    {
        string fullPath = GetPathFromAppData(relativePath);

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        await File.WriteAllBytesAsync(fullPath, data);
    }

    /// <summary>
    /// Saves bytes to the location. Creates any required directories to the file.
    /// </summary>
    /// <param name="relativePath">Relative path to the app data directory.</param>
    /// <param name="data">Data to save.</param>
    public static void SaveFileToLocation(string relativePath, byte[] data)
    {
        string fullPath = GetPathFromAppData(relativePath);

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        File.WriteAllBytes(fullPath, data);
    }

    /// <summary>
    /// Deletes files from the app data directory.
    /// </summary>
    /// <param name="relativePath">Relative path to the app data directory.</param>
    /// <returns>Whether was any file destroyed at that path.</returns>
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
