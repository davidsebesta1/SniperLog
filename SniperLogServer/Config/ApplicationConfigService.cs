using System.Reflection;

namespace Config;

/// <summary>
/// Service for config files.
/// </summary>
public static class ApplicationConfigService
{
    private static Dictionary<Type, object> _configs = null;

    /// <summary>
    /// Inits the configuration service and loads all config files into cache.
    /// </summary>
    public static void Init()
    {
        try
        {
            var configTypes = Assembly.GetExecutingAssembly().GetTypes().Where(n => n.GetInterface("IConfig") != null && !n.IsAbstract && !n.IsInterface);

            _configs = new Dictionary<Type, object>(configTypes.Count());

            foreach (Type type in configTypes)
            {
                LoadConfig(type);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Gets the config instance.
    /// </summary>
    /// <typeparam name="T">Type of the config.</typeparam>
    /// <returns>Config file instance.</returns>
    public static T? GetConfig<T>() where T : IConfig
    {
        if (_configs == null) 
            return default;

        if (_configs.TryGetValue(typeof(T), out object? config))
            return (T)config;
       
        return default;
    }

    /// <summary>
    /// Loads the config 
    /// </summary>
    /// <param name="type"></param>
    public static void LoadConfig(Type type)
    {
        string name = (string)type.GetProperty("Name", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        string fullPath = Path.Combine(AppContext.BaseDirectory, "SLConfigs", name + ".yaml");
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

    public static void SaveConfig(object config) => SaveConfig(config, Path.Combine(AppContext.BaseDirectory, "SLConfigs", (string)config.GetType().GetProperty("Name", BindingFlags.Static | BindingFlags.Public).GetValue(null)));

    public static void SaveConfig(object config, string fullPath)
    {
        string dir = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        Type type = config.GetType();
        string serialized = YamlParser.Serializer.Serialize(Convert.ChangeType(config, config.GetType()));
        File.WriteAllText(fullPath, serialized);
    }
}