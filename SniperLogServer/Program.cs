using Config;
using SniperLogNetworkLibrary;
using SniperLogNetworkLibrary.Networking.Messages;
using SniperLogServer.Connection;
using SniperLogServer.Weather;
using System.Net;

namespace SniperLogServer
{
    public class Program
    {
        public static ConnectionHandler ConnectionHandler { get; private set; }

        public static void Main(string[] args)
        {
            ApplicationConfigService.Init();
            ConnectionHandler = new ConnectionHandler(8000, IPAddress.Any);
            ConnectionHandler.RegisterNetworkMessageHandler<ErrorMessage>(new NetworkMessageErrorService());
            ConnectionHandler.RegisterNetworkMessageHandler<WeatherRequestMessage>(WeatherHandler.Instance);

            Console.WriteLine(ApplicationConfigService.GetConfig<AppConfig>().WeatherApiKey);

            Console.WriteLine("Starting server...");
            ConnectionHandler.Start();
            //Console.WriteLine("Sending weather request..");

            //WeatherHandler.SendWeatherRequestAsync(50.486238d, 14.201412d);

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}