using SniperLogNetworkLibrary.Networking.Messages;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SniperLog.Services.ConnectionToServer
{
    public class ConnectionToDataServer
    {
        private string _hostName;
        public string HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                _hostName = value;
                Task.Run(TryResolveIp);
            }
        }
        public ushort Port;

        private IPAddress? _ipAddress;
        private TcpClient _tcpClient;

        private readonly Dictionary<int, Type> _idToNetMessageType = new Dictionary<int, Type>();

        public ConnectionToDataServer()
        {
            AutoRegisterMessages();
        }

        private void AutoRegisterMessages()
        {
            var msgTypes = typeof(INetworkMessage).Assembly.GetTypes().Where(n => n.GetInterface("INetworkMessage") != null);

            foreach (Type msgType in msgTypes)
            {
                int id = (int)msgType.GetProperty("ID").GetValue(null);

                _idToNetMessageType.Add(id, msgType);
            }
        }

        private async Task TryResolveIp()
        {
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(_hostName);
                if (addresses.Length > 0)
                {
                    _ipAddress = addresses[0];
                }
            }
            catch (Exception ex)
            {
                _ipAddress = null;
            }
        }

        public async Task<INetworkMessage?> SendRequest(INetworkMessage request)
        {
            try
            {
                if (!await TryOpen())
                {
                    return new ErrorMessage("Error has occured, connection couldnt be setup");
                }


                byte[] data = request.Serialize();
                NetworkStream stream = _tcpClient.GetStream();

                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();

                byte[] buffer = new byte[128];
                int bytesRead;

                BinaryReader reader = new BinaryReader(new MemoryStream(buffer));

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    int msgId = reader.ReadInt32();
                    return ResolveNetworkMessage(msgId, reader);
                }

            }
            catch (Exception ex)
            {
                return new ErrorMessage($"Error has occured {ex.Message}");
            }
            finally
            {
                TryClose();
            }

            return new ErrorMessage("General error");
        }

        private INetworkMessage? ResolveNetworkMessage(int msgId, BinaryReader reader)
        {
            try
            {
                if (!_idToNetMessageType.TryGetValue(msgId, out Type type))
                {
                    return new ErrorMessage($"Unable to deserialize network message: id: {msgId} is not registered");
                }

                MethodInfo? deserializeMethod = type.GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public);

                if (deserializeMethod == null)
                {
                    return new ErrorMessage($"Unable to find deserialization method");
                }

                return (INetworkMessage)deserializeMethod.Invoke(null, [reader, null]);

            }
            catch (Exception ex)
            {
                return new ErrorMessage($"Error has occured {ex.Message}");
            }
        }

        public async Task<bool> TryOpen()
        {
            if (_ipAddress == null || (_tcpClient != null && _tcpClient.Connected)) return false;

            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_ipAddress, Port);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool TryClose()
        {
            try
            {
                if (_tcpClient == null || !_tcpClient.Connected) return false;

                _tcpClient.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}