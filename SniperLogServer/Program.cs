using Config;
using SniperLogNetworkLibrary;
using SniperLogNetworkLibrary.Networking.Messages;
using SniperLogServer.Connection;
using SniperLogServer.Logging;
using SniperLogServer.Weather;
using System.Net;

namespace SniperLogServer
{
    public class Program
    {
        public static ConnectionHandler ConnectionHandler { get; private set; }

        public static async Task Main(string[] args)
        {
            ApplicationConfigService.Init();
            ConnectionHandler = new ConnectionHandler(8000, IPAddress.Any);
            ConnectionHandler.RegisterNetworkMessageHandler<ErrorMessage>(new NetworkMessageErrorService());
            ConnectionHandler.RegisterNetworkMessageHandler<WeatherRequestMessage>(WeatherHandler.Instance);

            await Logger.Log(ApplicationConfigService.GetConfig<AppConfig>().WeatherApiKey);

            await Logger.Log("Starting server...");
            ConnectionHandler.Start();
            await Logger.Log("Press any key to exit");
            Console.ReadLine();
        }
    }
}