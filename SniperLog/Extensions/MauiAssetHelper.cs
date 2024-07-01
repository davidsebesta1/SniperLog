namespace SniperLog.Extensions
{
    public static class MauiAssetHelper
    {
        public static async Task<string> ReadTextFileAsync(string filePath)
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filePath);
            using StreamReader reader = new StreamReader(fileStream);

            return await reader.ReadToEndAsync();
        }
    }
}