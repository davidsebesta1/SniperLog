using System.Net.Sockets;

namespace SniperLogNetworkLibrary.Networking.Messages
{
    public interface INetworkMessage
    {
        public static virtual int ID { get; }
        public abstract TcpClient Requestor { get; }

        public abstract byte[] Serialize();
        public static virtual INetworkMessage Deserialize(BinaryReader stream, TcpClient requestor = null) => throw new NotImplementedException();
    }
}