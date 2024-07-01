
namespace Config
{
    public sealed class AppConfig : IConfig
    {
        public static string Name => "ServerConfig";

        public string WeatherURL { get; set; } = "https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&units=metric";
        public string WeatherApiKey { get; set; }
    }
}