using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class Bullet : ObservableObject, IDataAccessObject, IEquatable<Bullet?>
    {
        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(FirearmCaliber), nameof(FirearmCaliber.ID))]
        [ObservableProperty]
        private int _caliber_ID;

        [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
        [ObservableProperty]
        private int _manufacturer_ID;

        [ObservableProperty]
        private double? _weightGrams;

        [ObservableProperty]
        private double? _bCG1;

        [ObservableProperty]
        private double? _bCG7;

        public Bullet(int iD, int caliberID, int manuId, double? weightGrams, double? bc1, double? bc7)
        {
            ID = iD;
            Caliber_ID = caliberID;
            Manufacturer_ID = manuId;
            WeightGrams = weightGrams;
            BCG1 = bc1;
            BCG7 = bc7;
        }

        public Bullet(int caliberID, int manuId, double? weightGrams, double? bc1, double? bc7) : this(-1, caliberID, manuId, weightGrams, bc1, bc7)
        {

        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new Bullet(row);
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                if (ID == -1)
                {
                    ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId, GetSqliteParams(false));
                    return ID;
                }
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery,
                       GetSqliteParams(true));
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<Bullet>>().AddOrUpdate(this);
            }
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<Bullet>>().Remove(this);
            }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Bullet);
        }

        public bool Equals(Bullet? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, WeightGrams, BCG1, BCG7);
        }

        public override string? ToString()
        {
            return WeightGrams + "g, BC1: " + BCG1 + ", BC7: " + BCG7;
        }
    }
}
