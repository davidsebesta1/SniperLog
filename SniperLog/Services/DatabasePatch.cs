
namespace SniperLog.Services
{
    public class DatabasePatch : IEquatable<DatabasePatch>
    {
        public readonly Version Version;
        public readonly string PatchQuery;

        public DatabasePatch(Version version, string patchQuery)
        {
            Version = version;
            PatchQuery = patchQuery;
        }

        public void ApplyPatch()
        {
            SqLiteDatabaseConnection.Instance.ExecuteNonQuery(PatchQuery);
        }

        public override bool Equals(object? obj)
        {
            return obj is DatabasePatch patch && Equals(patch);
        }

        public bool Equals(DatabasePatch other)
        {
            return Version == other.Version;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version, PatchQuery);
        }

        public static bool operator ==(DatabasePatch left, DatabasePatch right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DatabasePatch left, DatabasePatch right)
        {
            return !(left == right);
        }

        public override string? ToString()
        {
            return Version + " : " + PatchQuery;
        }
    }
}