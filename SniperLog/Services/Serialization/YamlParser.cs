using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SniperLog.Services.Serialization;

/// <summary>
/// Static class containing serializer and deserializer for Yaml.
/// </summary>
public static class YamlParser
{
    /// <summary>
    /// Yaml serializer.
    /// </summary>
    public static ISerializer Serializer { get; } = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).DisableAliases().IgnoreFields().Build();

    /// <summary>
    /// Yaml deserializer.
    /// </summary>
    public static IDeserializer Deserializer { get; } = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).IgnoreUnmatchedProperties().IgnoreFields().Build();
}