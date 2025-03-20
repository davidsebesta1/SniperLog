namespace SniperLog.Extensions;

/// <summary>
/// A class extending maui usage and allow easy access to xaml - cs stuff.
/// </summary>
public static class MauiExtensions
{
    /// <summary>
    /// Reads content of a packaged text file asynchronously.
    /// <see href="https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers?view=net-maui-9.0&tabs=windows">Source</see>
    /// </summary>
    /// <param name="filePath">Path to the file relative to Resources.</param>
    /// <returns>String representation of the file.</returns>
    public static async Task<string> ReadTextFileAsync(string filePath)
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filePath);
        using StreamReader reader = new StreamReader(fileStream);

        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Copies bundles file to cache directory for use by some APIs.
    /// </summary>
    /// <param name="filename">Bundles file name.</param>
    /// <returns>Absolute path to the temp location of the file.</returns>
    public static async Task<string> GetRawFilePathAsync(string filename)
    {
        using Stream stream = await FileSystem.OpenAppPackageFileAsync(filename);

        string tempPath = Path.Combine(FileSystem.CacheDirectory, filename);

        using FileStream fileStream = File.Create(tempPath);
        stream.CopyTo(fileStream);

        return tempPath;
    }

    /// <summary>
    /// Gets resource from resource dictionary of the MAUI app.
    /// </summary>
    /// <typeparam name="T">Type of the resource.</typeparam>
    /// <param name="name">Name of the resource in XAML file.</param>
    /// <param name="found">Whether the resource has been found.</param>
    /// <returns>Retrived object or its default value if it isn't found.</returns>
    public static T? GetResource<T>(string name, out bool found)
    {
        found = false;
        if (Application.Current.Resources.TryGetValue(name, out object val))
        {
            found = true;
            return (T)val;
        }

        return default;
    }
}
