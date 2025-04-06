using SniperLog.Config.Interfaces;

namespace SniperLog.Config;

/// <summary>
/// Version control config.
/// </summary>
public class VersionControl : IConfig
{
    /// <inheritdoc/>
    public static string Name => "VersionControl";

    /// <summary>
    /// Value whether this is the first launch ever of the app. Used to initialize database.
    /// </summary>
    public bool FirstLaunchEver { get; set; } = true;
}
