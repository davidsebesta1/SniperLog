using SniperLogNetworkLibrary.Networking;
using SniperLogNetworkLibrary.Networking.Messages;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SniperLogServer.Connection
{
    public class ConnectionHandler
    {
        public ushort ListeningPort { get; private set; }
        public IPAddress ListeningAddress { get; private set; }

        private bool _running = false;

        private readonly TcpListener _listener;
        private readonly List<TcpClient> _connectedClients;
        private readonly object _lock = new object();

        private readonly Thread _loopThread;

        private readonly Dictionary<int, Type> _networkMessagesByType = new Dictionary<int, Type>();
        private readonly Dictionary<int, IServerNetworkMessageHandler> _networkMessageHandlers = new Dictionary<int, IServerNetworkMessageHandler>();

        public ConnectionHandler(ushort port, IPAddress address)
        {
            ListeningPort = port;
            ListeningAddress = address;

            _listener = new TcpListener(ListeningAddress, ListeningPort);
            _connectedClients = new List<TcpClient>();
            _loopThread = new Thread(LoopThread);
        }

        public void Start()
        {
            _running = true;
            _listener.Start();
            _loopThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
        }

        private async void LoopThread()
        {
            while (_running)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    lock (_lock)
                    {
                        _connectedClients.Add(client);
                        Console.WriteLine((client.Client.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString() + " has connected");
                    }

                    await Task.Run(() =>
                    {
                        ClientLoop(client);
                    });
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.ToString());
                }
            }
        }

        private async Task ClientLoop(TcpClient client)
        {
            try
            {
                using (client)
                {
                    while (client.Connected)
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead;
                            BinaryReader reader = new BinaryReader(new MemoryStream(buffer));

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                Console.WriteLine("Read: " + bytesRead + "bytes");
                                int id = reader.ReadInt32();
                                Console.WriteLine("Received ID: " + id);
                                ResolveNetworkMessage(client, id, reader);

                                reader.BaseStream.Position = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine(client + " has disconnected");

                lock (_lock)
                {
                    _connectedClients.Remove(client);
                }
            }
        }

        public async void SendDataToClient(TcpClient client, INetworkMessage msg)
        {
            byte[] data = msg.Serialize();
            Console.WriteLine("Sending: " + msg.ToString() + " to " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.MapToIPv4().ToString());

            try
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to client: {ex.Message}");

                lock (_lock)
                {
                    _connectedClients.Remove(client);
                }
            }
        }

        public bool RegisterNetworkMessageHandler<T>(IServerNetworkMessageHandler handler) where T : INetworkMessage
        {
            if (_networkMessageHandlers.ContainsKey(T.ID)) return false;

            _networkMessageHandlers.Add(T.ID, handler);
            _networkMessagesByType.Add(T.ID, typeof(T));
            return true;
        }

        public void ResolveNetworkMessage(TcpClient client, int id, BinaryReader reader)
        {
            try
            {
                if (!_networkMessagesByType.TryGetValue(id, out Type type))
                {
                    return;
                }
                if (!_networkMessageHandlers.TryGetValue(id, out IServerNetworkMessageHandler handler))
                {
                    _networkMessageHandlers[0].HandleMessage(client, new ErrorMessage("Message handler not found"));
                }

                MethodInfo mInfo = type.GetMethod("Deserialize");
                INetworkMessage msg = mInfo.Invoke(null, [reader, client]) as INetworkMessage;
                handler.HandleMessage(client, msg);

            }
            catch (Exception ex)
            {
                _networkMessageHandlers[0].HandleMessage(client, new ErrorMessage(ex.ToString()));
            }
        }
    }
}