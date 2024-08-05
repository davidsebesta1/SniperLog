using Config;
using Newtonsoft.Json;
using SniperLogNetworkLibrary;
using SniperLogNetworkLibrary.Networking;
using SniperLogNetworkLibrary.Networking.Messages;
using SniperLogServer.Connection;
using System.Net.Sockets;

namespace SniperLogServer.Weather
{
    public class WeatherHandler : IServerNetworkMessageHandler
    {
        private readonly Dictionary<LatLongStruct, WeatherResponseMessage> _weatherCache = new Dictionary<LatLongStruct, WeatherResponseMessage>();

        private static WeatherHandler _instance;
        public static WeatherHandler Instance
        {
            get
            {
                return _instance ??= new WeatherHandler();
            }
        }

        public async Task<WeatherResponseMessage> GetWeather(WeatherRequestMessage msg) => await GetWeather(msg.Latitude, msg.Longitude);

        public async Task<WeatherResponseMessage> GetWeather(double latitude, double longitude)
        {
            LatLongStruct latLongStruct = new LatLongStruct(latitude, longitude);

            if (_weatherCache.TryGetValue(latLongStruct, out WeatherResponseMessage weather))
            {
                if ((DateTime.UtcNow - weather.TimeTaken).Value.TotalMinutes > 5)
                {
                    weather = await DeserializeFromJson(await SendWeatherRequestAsync(latitude, longitude));
                    _weatherCache[latLongStruct] = weather;
                }
                return weather;
            }

            weather = await DeserializeFromJson(await SendWeatherRequestAsync(latitude, longitude));
            _weatherCache.Add(latLongStruct, weather);
            return weather;
        }

        public async Task<WeatherResponseMessage> DeserializeFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            dynamic jsonObject = JsonConvert.DeserializeObject(json);
            string clouds = jsonObject.weather[0].main;

            double temperature = jsonObject.main.temp;
            ushort pressure = jsonObject.main.pressure;
            byte huminidy = jsonObject.main.humidity;

            byte windSpeed = jsonObject.wind.speed;
            ushort directionDegrees = jsonObject.wind.deg;

            return new WeatherResponseMessage(DateTime.UtcNow, clouds, temperature, pressure, huminidy, windSpeed, directionDegrees);
        }

        public async Task<string> SendWeatherRequestAsync(double latitude, double longitude)
        {
            string formattedUrl = string.Format(ApplicationConfigService.GetConfig<AppConfig>().WeatherURL, latitude.ToString(), longitude.ToString(), ApplicationConfigService.GetConfig<AppConfig>().WeatherApiKey);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage message = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, formattedUrl));
                if (message.IsSuccessStatusCode)
                {
                    await Console.Out.WriteLineAsync(await message.Content.ReadAsStringAsync());
                    return await message.Content.ReadAsStringAsync();
                }
                else
                {
                    await Console.Out.WriteLineAsync("Error sending request: " + message.ReasonPhrase);
                }
            }

            return string.Empty;
        }

        public async void HandleMessage(TcpClient client, INetworkMessage message)
        {
            WeatherRequestMessage msg = (WeatherRequestMessage)message;
            WeatherResponseMessage response = await GetWeather(msg);
            Program.ConnectionHandler.SendDataToClient(client, response);
        }
    }
}