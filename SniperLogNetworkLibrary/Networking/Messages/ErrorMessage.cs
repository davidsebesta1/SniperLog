using SniperLogNetworkLibrary.CommonLib;
using System.Net.Sockets;
using System.Text;

namespace SniperLogNetworkLibrary.Networking.Messages
{
    public readonly struct ErrorMessage : INetworkMessage
    {
        public static int ID => 0;

        public TcpClient Requestor => _requester;
        private readonly TcpClient _requester;

        public readonly string Message;

        public ErrorMessage(string message, TcpClient requester = null)
        {
            _requester = requester;
            Message = message;
        }

        public static INetworkMessage Deserialize(BinaryReader stream, TcpClient requester = null)
        {
            string msg = Encoding.UTF8.GetString(stream.ReadBytes(stream.ReadInt32()));
            return new ErrorMessage(msg, requester);
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>(sizeof(int) + Message.Length);
            bytes.AddRange(BitConverter.GetBytes(Message.Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(Message));

            return bytes.GetInternalArray();
        }

        public override string? ToString()
        {
            return "Error message: " + Message;
        }
    }
}