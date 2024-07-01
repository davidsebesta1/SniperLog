using SniperLogNetworkLibrary.Networking.Messages;
using System.Net.Sockets;

namespace SniperLogNetworkLibrary.Networking
{
    public interface IServerNetworkMessageHandler
    {
        public abstract void HandleMessage(TcpClient client, INetworkMessage message);
    }
}