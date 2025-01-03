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
        private int _caliberID;

        [ObservableProperty]
        private double _weightGrams;

        [ObservableProperty]
        private double _ballisticCoeficient;

        public Bullet(int iD, int caliberID, double weightGrams, double ballisticCoeficient)
        {
            ID = iD;
            CaliberID = caliberID;
            WeightGrams = weightGrams;
            BallisticCoeficient = ballisticCoeficient;
        }

        public Bullet(int caliberID, double weightGrams, double ballisticCoeficient) : this(-1, caliberID, weightGrams, ballisticCoeficient)
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
            return HashCode.Combine(ID, WeightGrams, BallisticCoeficient);
        }

        public override string? ToString()
        {
            return WeightGrams + "g, BC: " + BallisticCoeficient;
        }
    }
}
