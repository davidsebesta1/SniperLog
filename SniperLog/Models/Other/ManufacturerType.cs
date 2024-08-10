using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class ManufacturerType : IDataAccessObject, ICsvProcessable
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }

        public static string CsvHeader => "Name";

        #endregion

        #region Ctors

        public ManufacturerType(int iD, string name)
        {
            ID = iD;
            Name = name;
        }

        public ManufacturerType(string name) : this(-1, name)
        {

        }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new ManufacturerType(row);
        }

        public async Task<int> SaveAsync()
        {
            try
            {

                if (ID == -1)
                {
                    ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId,
                         GetSqliteParams(false));
                    return ID;
                }
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery,
                       GetSqliteParams(true));
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<ManufacturerType>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<ManufacturerType>>().Remove(this);
            }
        }

        #endregion

        #region CSV

        public string SerializeToCsvRow()
        {
            return Name;
        }

        public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
        {
            return new ManufacturerType(row);
        }

        #endregion
    }
}
