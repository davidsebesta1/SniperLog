namespace SniperLog.Services;

/// <summary>
/// Static services helper class to get the service from the service provider.
/// </summary>
public static class ServicesHelper
{
    /// <summary>
    /// Service provider.
    /// </summary>
    public static IServiceProvider? Services { get; private set; }

    /// <summary>
    /// Init method.
    /// </summary>
    /// <param name="serviceProvider">Service provider that is used.</param>
    public static void Init(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
    }

    /// <summary>
    /// Gets the service via generic parameter.
    /// </summary>
    /// <typeparam name="T">Service type.</typeparam>
    /// <returns>The service or <see langword="null"/> if it doesn't exists.</returns>
    public static T? GetService<T>()
    {
        if (Services is null)
            return default;

        return Services.GetService<T>();
    }

    /// <summary>
    /// Gets the service via <see cref="Type"/>.
    /// </summary>
    /// <param name="type">Type of the service class.</param>
    /// <returns>The service or <see langword="null"/> if it doesn't exists.</returns>
    public static object? GetService(Type type)
    {
        if (Services is null)
            return default;

        return Services.GetService(type);
    }
}