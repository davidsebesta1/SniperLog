using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Models
{
    public partial class MuzzleVelocity : ObservableObject, IDataAccessObject, IEquatable<MuzzleVelocity?>
    {
        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(Ammunition), nameof(Ammunition.ID))]
        [ObservableProperty]
        private int _ammoID;

        [ForeignKey(typeof(Firearm), nameof(Firearm.ID))]
        [ObservableProperty]
        private int _firearmID;

        [ObservableProperty]
        private double _velocityMS;

        public MuzzleVelocity(int iD, int ammoID, int firearmID, double velocityMS)
        {
            ID = iD;
            AmmoID = ammoID;
            FirearmID = firearmID;
            VelocityMS = velocityMS;
        }

        public MuzzleVelocity(int ammoID, int firearmID, double velocityMS) : this(-1, ammoID, firearmID, velocityMS)
        {

        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new MuzzleVelocity(row);
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
                ServicesHelper.GetService<DataCacherService<MuzzleVelocity>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<MuzzleVelocity>>().Remove(this);
            }
        }



        public override bool Equals(object? obj)
        {
            return Equals(obj as MuzzleVelocity);
        }

        public bool Equals(MuzzleVelocity? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, VelocityMS);
        }

        public override string? ToString()
        {
            return VelocityMS + " m/s";
        }
    }
}
