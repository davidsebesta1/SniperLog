
namespace SniperLog.Config.Interfaces;

/// <summary>
/// Interface for all configuration files of the program.
/// </summary>
public interface IConfig
{
    /// <summary>
    /// Name of the configuration file without extension.
    /// </summary>
    public static abstract string Name { get; }
}
