using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class ShootingRecordImage : ObservableObject, IDataAccessObject, IImageSaveable, IEquatable<ShootingRecordImage?>
    {
        public string ImageSavePath
        {
            get
            {
                return Path.Combine("Data", "ShootingRanges", ID.ToString(), "backgroundimage.png");
            }
        }

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        [ForeignKey(typeof(ShootingRecord), nameof(ShootingRecord.ID))]
        private int _shootingRecord_ID;

        #endregion

        #region Ctor

        public ShootingRecordImage(int iD, int shootingRecord_ID)
        {
            ID = iD;
            _shootingRecord_ID = shootingRecord_ID;
        }

        public ShootingRecordImage(int shootingRecord_ID) : this(-1, shootingRecord_ID) { }

        #endregion

        #region DAO

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new ShootingRecordImage(row);
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
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery, GetSqliteParams(true));
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<ShootingRecordImage>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<ShootingRecordImage>>().Remove(this);
            }
        }

        #endregion

        #region Object

        public override bool Equals(object? obj)
        {
            return Equals(obj as ShootingRecordImage);
        }

        public bool Equals(ShootingRecordImage? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(ShootingRecordImage? left, ShootingRecordImage? right)
        {
            return EqualityComparer<ShootingRecordImage>.Default.Equals(left, right);
        }

        public static bool operator !=(ShootingRecordImage? left, ShootingRecordImage? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
