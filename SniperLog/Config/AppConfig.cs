using SniperLog.Config.Interfaces;

namespace SniperLog.Config;

/// <summary>
/// Misc app configuration.
/// </summary>
public sealed class AppConfig : IConfig
{
    /// <inheritdoc/>
    public static string Name => "MainConfig";

    /// <summary>
    /// List of applied patches to the db.
    /// </summary>
    public List<Version> AppliedPatches = new List<Version>();

    /// <summary>
    /// Host name of the weather server to connect to.
    /// </summary>
    public string ServerHostname { get; set; } = "dev.spsejecna.net";

    /// <summary>
    /// Port to connect to for the <see cref="ServerHostname"/>.
    /// </summary>
    public ushort ServerPort { get; set; } = 8000;
}
