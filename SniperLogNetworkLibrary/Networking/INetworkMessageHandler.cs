using SniperLogNetworkLibrary.Networking.Messages;

namespace SniperLogNetworkLibrary
{
    public interface INetworkMessageHandler
    {
        public abstract void HandleMessage(INetworkMessage message);
    }
}