using SniperLogNetworkLibrary.Networking.Messages;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SniperLog.Services.ConnectionToServer
{
    public class ConnectionToDataServer
    {
        public const int ConnectionTimeoutErrorCode = 110;

        public ushort Port;

        public IPAddress? IpAddress;
        private TcpClient _tcpClient;

        private Dictionary<int, Type> _idToNetMessageType;

        public ConnectionToDataServer()
        {
            AutoRegisterMessages();
        }

        private void AutoRegisterMessages()
        {
            var msgTypes = typeof(INetworkMessage).Assembly.GetTypes().Where(n => n.GetInterface("INetworkMessage") != null);
            _idToNetMessageType = new Dictionary<int, Type>(msgTypes.Count());

            foreach (Type msgType in msgTypes)
            {
                int id = (int)msgType.GetProperty("ID").GetValue(null);

                _idToNetMessageType.Add(id, msgType);
            }
        }

        /*
        private async Task TryResolveIp()
        {
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(_hostName);
                if (addresses.Length > 0)
                {
                    IpAddress = addresses[0];
                }
            }
            catch (Exception ex)
            {
                IpAddress = null;
            }
        }
        */

        public async Task<INetworkMessage?> SendRequest(INetworkMessage request)
        {
            try
            {
                if (!await TryOpen())
                {
                    throw new TimeoutException();
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == ConnectionTimeoutErrorCode)
                {
                    return null;
                }

                return new ErrorMessage($"Error has occured: {e.Message}");
            }

            try
            {
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
                return new ErrorMessage($"Error has occured: {ex.Message}");
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
            if (IpAddress == null)
            {
                throw new ArgumentNullException("IP Address is null");
            }

            if (_tcpClient != null && _tcpClient.Connected)
            {
                throw new Exception("TCP Client is already connected, but system is trying to connect again");
            }

            _tcpClient = new TcpClient();
            if (!_tcpClient.ConnectAsync(IpAddress, Port).Wait(5))
            {
                throw new TimeoutException("Connection timeout");
            }

            return true;
        }

        public bool TryClose()
        {
            try
            {
                if (_tcpClient == null || !_tcpClient.Connected)
                    return false;

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