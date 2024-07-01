using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SniperLog.Services.Serialization
{
    public static class YamlParser
    {
        public static ISerializer Serializer { get; } = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).DisableAliases().IgnoreFields().Build();

        public static IDeserializer Deserializer { get; } = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).IgnoreUnmatchedProperties().IgnoreFields().Build();
    }
}
