using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class SightClickType : ObservableObject, IDataAccessObject, ICsvProcessable, IEquatable<SightClickType?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        public string ClickTypeName { get; set; }

        public static string CsvHeader => "ClickTypeName";

        #endregion

        #region CSV

        public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
        {
            return new SightClickType(row);
        }

        public string SerializeToCsvRow()
        {
            return ClickTypeName;
        }

        #endregion

        #region Constructor

        public SightClickType(int id, string name)
        {
            ID = id;
            ClickTypeName = name;
        }

        public SightClickType(string name) : this(-1, name) { }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new SightClickType(row);
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;

            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<SightClickType>>().Remove(this);
            }
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
                else
                {
                    return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery, GetSqliteParams(true));
                }
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<SightClickType>>().AddOrUpdate(this);
            }
        }

        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as SightClickType);
        }

        public bool Equals(SightClickType? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SightClickType? left, SightClickType? right)
        {
            return EqualityComparer<SightClickType>.Default.Equals(left, right);
        }

        public static bool operator !=(SightClickType? left, SightClickType? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
