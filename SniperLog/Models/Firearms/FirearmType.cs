using Microsoft.Data.Sqlite;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class FirearmType : IDataAccessObject, ICsvProcessable, IEquatable<FirearmType?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        public string TypeName { get; set; }

        public static string CsvHeader => "TypeName";

        #endregion

        #region Constructors

        public FirearmType(int iD, string name)
        {
            ID = iD;
            TypeName = name;
        }

        public FirearmType(string name) : this(-1, name)
        {

        }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new FirearmType(row);
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
                ServicesHelper.GetService<DataCacherService<FirearmType>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<FirearmType>>().Remove(this);
            }
        }


        #endregion

        #region CSV

        public static ICsvProcessable DeserializeFromCsvRow(string row)
        {
            return new FirearmType(row);
        }

        public string SerializeToCsvRow()
        {
            return TypeName;
        }


        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as FirearmType);
        }

        public bool Equals(FirearmType? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FirearmType? left, FirearmType? right)
        {
            return EqualityComparer<FirearmType>.Default.Equals(left, right);
        }

        public static bool operator !=(FirearmType? left, FirearmType? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
