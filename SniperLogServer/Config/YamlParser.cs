using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Config
{
    public static class YamlParser
    {
        public static ISerializer Serializer { get; } = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).DisableAliases().IgnoreFields().Build();

        public static IDeserializer Deserializer { get; } = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).IgnoreUnmatchedProperties().IgnoreFields().Build();
    }
}
