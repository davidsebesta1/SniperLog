using SniperLog.Config.Interfaces;
using SniperLog.Extensions;
using SniperLog.Services.Serialization;
using System.Reflection;

namespace SniperLog.Services.Configuration
{
    public static class ApplicationConfigService
    {
        private static Dictionary<Type, object> _configs = null;

        public static T? GetConfig<T>() where T : IConfig
        {
            if (_configs == null) 
                return default;

            if (_configs.TryGetValue(typeof(T), out object? config))
            {
                return (T)config;
            }

            return default;
        }

        public static void Init()
        {
            var configTypes = typeof(ApplicationConfigService).Assembly.GetTypes().Where(n => n.GetInterface("IConfig") != null && !n.IsAbstract && !n.IsInterface);

            _configs = new Dictionary<Type, object>(configTypes.Count());

            foreach (Type type in configTypes)
            {
                LoadConfig(type);
            }
        }

        public static void LoadConfig(Type type)
        {
            string name = (string)type.GetProperty("Name", BindingFlags.Static | BindingFlags.Public).GetValue(null);

            string fullPath = AppDataFileHelper.GetPathFromAppData(name + ".yaml");

            object config = null;
            try
            {
                if (!File.Exists(fullPath))
                {
                    config = Convert.ChangeType(Activator.CreateInstance(type), type);
                    SaveConfig(config, fullPath);
                    return;
                }

                config = YamlParser.Deserializer.Deserialize(File.ReadAllText(fullPath), type);
            }
            catch (Exception ex)
            {
                config = Convert.ChangeType(Activator.CreateInstance(type), type);
                SaveConfig(config, fullPath);
            }
            finally
            {
                _configs.Add(type, config);
            }
        }

        public static void SaveConfig(object config) => SaveConfig(config, AppDataFileHelper.GetPathFromAppData((string)config.GetType().GetProperty("Name", BindingFlags.Static | BindingFlags.Public).GetValue(null)));

        public static void SaveConfig(object config, string fullPath)
        {
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Type type = config.GetType();
            string serialized = YamlParser.Serializer.Serialize(Convert.ChangeType(config, config.GetType()));
            File.WriteAllText(fullPath.EndsWith(".yaml") ? fullPath : fullPath + ".yaml", serialized);
        }
    }
}
