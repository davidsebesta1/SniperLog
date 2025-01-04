using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Models
{
    public partial class Ammunition : ObservableObject, IDataAccessObject, IEquatable<Ammunition?>
    {
        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(Bullet), nameof(Bullet.ID))]
        [ObservableProperty]
        private int _bullet_ID;

        [ObservableProperty]
        private double _totalLengthMm;

        [ObservableProperty]
        private double _gunpowderAmountGrams;

        public Ammunition(int iD, int bulletID, double totalLengthMm, double gunpowderAmountGrams)
        {
            ID = iD;
            Bullet_ID = bulletID;
            TotalLengthMm = totalLengthMm;
            GunpowderAmountGrams = gunpowderAmountGrams;
        }

        public Ammunition(int bulletID, double totalLengthMm, double gunpowderAmountGrams) : this(-1, bulletID, totalLengthMm, gunpowderAmountGrams)
        {

        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new Ammunition(row);
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
                ServicesHelper.GetService<DataCacherService<Ammunition>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<Ammunition>>().Remove(this);
            }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Ammunition);
        }

        public bool Equals(Ammunition? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, TotalLengthMm, GunpowderAmountGrams);
        }

        public override string? ToString()
        {
            return TotalLengthMm + "mm";
        }
    }
}
