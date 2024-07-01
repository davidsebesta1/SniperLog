namespace SniperLogServer.Weather
{
    public readonly struct LatLongStruct : IEquatable<LatLongStruct>
    {
        public readonly double Latitude;
        public readonly double Longitude;

        public LatLongStruct(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public bool Equals(LatLongStruct other, double tolerance = 0.000001)
        {
            return Math.Abs(Latitude - other.Latitude) < tolerance && Math.Abs(Longitude - other.Longitude) < tolerance;
        }

        public override bool Equals(object? obj)
        {
            return obj is LatLongStruct @struct && Equals(@struct);
        }

        public bool Equals(LatLongStruct other)
        {
            return Equals(other, tolerance: 0.0001);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }

        public static bool operator ==(LatLongStruct left, LatLongStruct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LatLongStruct left, LatLongStruct right)
        {
            return !(left == right);
        }
    }
}
