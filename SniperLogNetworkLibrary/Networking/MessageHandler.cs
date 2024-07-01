
using SniperLogNetworkLibrary.Networking.Messages;

namespace SniperLogNetworkLibrary
{
    public static class MessageHandler<T> where T : INetworkMessage
    {
        private static Action<T> OnMessageReceived;

        public static void RegisterHandler(Action<T> del)
        {
            OnMessageReceived = del;
        }

        public static void Invoke(T message)
        {
            OnMessageReceived?.Invoke(message);
        }
    }
}