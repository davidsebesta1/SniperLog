using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace SniperLog.Services
{
    public static class YamlParser
    {
        public static ISerializer Serializer { get; } = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).DisableAliases().IgnoreFields().Build();

        public static IDeserializer Deserializer { get; } = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).IgnoreUnmatchedProperties().IgnoreFields().Build();
    }
}
