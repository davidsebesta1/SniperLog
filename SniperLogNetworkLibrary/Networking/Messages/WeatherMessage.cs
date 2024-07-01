using System.Net.Sockets;
using System.Text;
using SniperLogNetworkLibrary.CommonLib;
using SniperLogNetworkLibrary.Networking.Messages;

namespace SniperLogNetworkLibrary
{
    public readonly struct WeatherRequestMessage : INetworkMessage
    {
        public static int ID => 1;

        public TcpClient Requestor => _requester;
        private readonly TcpClient _requester;

        public readonly double Latitude;
        public readonly double Longitude;

        public WeatherRequestMessage(double latitude, double longitude, TcpClient requester = null)
        {
            _requester = requester;
            Latitude = latitude;
            Longitude = longitude;
        }

        public static INetworkMessage Deserialize(BinaryReader stream, TcpClient requestor = null)
        {
            double latitude = stream.ReadDouble();
            double longitude = stream.ReadDouble();

            return new WeatherRequestMessage(latitude, longitude, requestor);
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>(sizeof(int) + sizeof(double) * 2);
            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.AddRange(BitConverter.GetBytes(Latitude));
            bytes.AddRange(BitConverter.GetBytes(Longitude));

            return bytes.GetInternalArray();
        }

        public override string? ToString()
        {
            return $"Latitude: {Latitude}, Longitude: {Longitude}";
        }
    }

    public readonly struct WeatherResponseMessage : INetworkMessage
    {
        public static int ID => -1;

        public TcpClient Requestor => _requester;
        private readonly TcpClient _requester;

        /// <summary>
        /// Utc time of time taken of this weather snapshot
        /// </summary>
        public readonly DateTime TimeTaken { get; }

        public readonly string Clouds { get; }

        public readonly double Temperature { get; }
        public readonly ushort Pressure { get; }
        public readonly byte Humidity { get; }

        public readonly byte WindSpeed { get; }
        public readonly ushort DirectionDegrees { get; }

        public WeatherResponseMessage(DateTime timeTaken, string clouds, double temperature, ushort pressure, byte humidity, byte windSpeed, ushort directionDegrees, TcpClient requester = null)
        {
            _requester = requester;
            TimeTaken = timeTaken;
            Clouds = clouds;
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            WindSpeed = windSpeed;
            DirectionDegrees = directionDegrees;
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>(sizeof(int) + sizeof(byte) + Clouds.Length + sizeof(double) + sizeof(ushort) * 2 + sizeof(byte) * 2 + sizeof(long));

            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.Add((byte)Clouds.Length);
            bytes.AddRange(Encoding.UTF8.GetBytes(Clouds));
            bytes.AddRange(BitConverter.GetBytes(Temperature));
            bytes.AddRange(BitConverter.GetBytes(Pressure));
            bytes.Add(Humidity);
            bytes.Add(WindSpeed);
            bytes.AddRange(BitConverter.GetBytes(DirectionDegrees));
            bytes.AddRange(BitConverter.GetBytes(TimeTaken.ToBinary()));

            return bytes.GetInternalArray();
        }

        public static INetworkMessage Deserialize(BinaryReader stream, TcpClient requestor = null)
        {
            string clouds = new string(stream.ReadChars(stream.ReadByte()));
            double temp = stream.ReadDouble();
            ushort pressure = stream.ReadUInt16();
            byte huminidy = stream.ReadByte();
            byte windSpeed = stream.ReadByte();
            ushort windDir = stream.ReadUInt16();
            DateTime timeTaken = DateTime.FromBinary(stream.ReadInt64());

            return new WeatherResponseMessage(timeTaken, clouds, temp, pressure, huminidy, windSpeed, windDir, requestor);
        }

        public override string? ToString()
        {
            return $"Time taken: {TimeTaken}, Clouds: {Clouds}, Tmp: {Temperature}, Pressure: {Pressure}, Humidity: {Humidity}, WSpeed: {WindSpeed}, WDir: {DirectionDegrees}";
        }
    }
}