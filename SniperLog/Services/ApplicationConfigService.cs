using SniperLog.Config;

namespace SniperLog.Services
{
    public static class ApplicationConfigService
    {
        private static AppConfig _config;
        private static readonly string _configName = "Config.yaml";
        private static readonly string _fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, _configName);

        public static AppConfig Config
        {
            get
            {
                return _config;
            }
        }

        public static void Init()
        {
            if (_config != null) return;

            LoadConfig();
        }

        public static void LoadConfig()
        {
            try
            {

                if (!File.Exists(_fullPath))
                {
                    _config = Activator.CreateInstance(typeof(AppConfig)) as AppConfig;
                    SaveConfig();
                    return;
                }

                _config = YamlParser.Deserializer.Deserialize<AppConfig>(_fullPath);
            }
            catch (Exception ex)
            {
                _config = Activator.CreateInstance(typeof(AppConfig)) as AppConfig;
                SaveConfig();
            }
        }

        public static void SaveConfig()
        {
            File.WriteAllText(_fullPath, YamlParser.Serializer.Serialize(_config));
        }
    }
}
