using SniperLogNetworkLibrary.Networking;
using SniperLogNetworkLibrary.Networking.Messages;
using System.Net.Sockets;

namespace SniperLogServer
{
    public class NetworkMessageErrorService : IServerNetworkMessageHandler
    {
        public async void HandleMessage(TcpClient client, INetworkMessage message)
        {
            Program.ConnectionHandler.SendDataToClient(client, message);
        }
    }
}