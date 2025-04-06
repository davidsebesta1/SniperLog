using SniperLogNetworkLibrary.Networking.Messages;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SniperLog.Services.ConnectionToServer;

/// <summary>
/// Service to connect to the weather data service.
/// </summary>
public class ConnectionToDataServer
{
    /// <summary>
    /// Connection timeout error code.
    /// </summary>
    public const int ConnectionTimeoutErrorCode = 110;

    /// <summary>
    /// Port to connect to.
    /// </summary>
    public ushort Port;

    /// <summary>
    /// IP address to connect to.
    /// </summary>
    public IPAddress? IpAddress;

    private TcpClient _tcpClient;
    private Dictionary<int, Type> _idToNetMessageType;

    /// <summary>
    /// Ctor.
    /// </summary>
    public ConnectionToDataServer()
    {
        AutoRegisterMessages();
    }

    /// <summary>
    /// Resolved hostname to IP.
    /// </summary>
    /// <param name="hostName">Hostname.</param>
    /// <returns>IP Address.</returns>
    public static async Task<IPAddress?> TryResolveIp(string hostName)
    {
        try
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(hostName);
            if (addresses.Length > 0)
                return addresses[0];
        }
        catch
        {
            return null;
        }

        return null;
    }

    /// <summary>
    /// Attempts to open the connection.
    /// </summary>
    /// <returns>Whethet it was connected succesfully.</returns>
    public bool TryOpen()
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

    /// <summary>
    /// Attenpts to close the connection.
    /// </summary>
    /// <returns>Whether the connection was closed succesfully.</returns>
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

    /// <summary>
    /// Sends a request to the server.
    /// </summary>
    /// <param name="request">Request message.</param>
    /// <returns>Response form the server.</returns>
    public async Task<INetworkMessage?> SendRequest(INetworkMessage request)
    {
        try
        {
            if (!TryOpen())
                throw new TimeoutException();
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == ConnectionTimeoutErrorCode)
                return null;

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
}