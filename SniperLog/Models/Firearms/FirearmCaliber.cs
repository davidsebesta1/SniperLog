using Microsoft.Data.Sqlite;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class FirearmCaliber : IDataAccessObject, ICsvProcessable, IEquatable<FirearmCaliber?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        public string Caliber { get; set; }

        public static string CsvHeader => "Name";

        #endregion

        #region Constructors

        public FirearmCaliber(int iD, string name)
        {
            ID = iD;
            Caliber = name;
        }

        public FirearmCaliber(string name) : this(-1, name)
        {

        }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new FirearmCaliber(row);
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                if (ID == -1)
                {
                    ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId, GetSqliteParams(true));
                    return ID;
                }
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery, GetSqliteParams(false));
            }
            finally
            {
                await ServicesHelper.GetService<DataCacherService<FirearmCaliber>>().AddOrUpdateAsync(this);
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
                await ServicesHelper.GetService<DataCacherService<FirearmCaliber>>().RemoveAsync(this);
            }
        }

        #endregion

        #region CSV

        public static ICsvProcessable DeserializeFromCsvRow(string row)
        {
            return new FirearmCaliber(row);
        }

        public string SerializeToCsvRow()
        {
            return Caliber;
        }

        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as FirearmCaliber);
        }

        public bool Equals(FirearmCaliber? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FirearmCaliber? left, FirearmCaliber? right)
        {
            return EqualityComparer<FirearmCaliber>.Default.Equals(left, right);
        }

        public static bool operator !=(FirearmCaliber? left, FirearmCaliber? right)
        {
            return !(left == right);
        }

        #endregion
    }
}