using YamlDotNet.Serialization;

/* Unmerged change from project 'SniperLog (net8.0-windows10.0.19041.0)'
Before:
using YamlDotNet.Serialization;
After:
using YamlDotNet.Serialization.NamingConventions;
*/

/* Unmerged change from project 'SniperLog (net8.0-android)'
Before:
using YamlDotNet.Serialization;
After:
using YamlDotNet.Serialization.NamingConventions;
*/

/* Unmerged change from project 'SniperLog (net8.0-ios)'
Before:
using YamlDotNet.Serialization;
After:
using YamlDotNet.Serialization.NamingConventions;
*/
using YamlDotNet.Serialization.NamingConventions;

namespace SniperLog.Services
{
    public static class YamlParser
    {
        public static ISerializer Serializer { get; } = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).DisableAliases().IgnoreFields().Build();

        public static IDeserializer Deserializer { get; } = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).IgnoreUnmatchedProperties().IgnoreFields().Build();
    }
}
