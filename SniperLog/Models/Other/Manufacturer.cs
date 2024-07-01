using Microsoft.Data.Sqlite;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class Manufacturer : IDataAccessObject, ICsvProcessable, IEquatable<Manufacturer?>
    {

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(Country), nameof(Country.ID))]
        public int Country_ID { get; set; }

        public string Name { get; set; }

        public static string CsvHeader => "Name,CountryCode";

        #endregion

        #region Constructors

        public Manufacturer(int iD, int country_ID, string name)
        {
            ID = iD;
            Country_ID = country_ID;
            Name = name;
        }

        public Manufacturer(int country_ID, string name) : this(-1, country_ID, name)
        {

        }

        #endregion

        #region DAO methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new Manufacturer(row);
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
                await ServicesHelper.GetService<DataCacherService<Manufacturer>>().AddOrUpdateAsync(this);
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
                await ServicesHelper.GetService<DataCacherService<Manufacturer>>().RemoveAsync(this);
            }
        }

        #endregion

        #region CSV

        public string SerializeToCsvRow()
        {
            return string.Join(',', Name, ReferencedCountry.Code);
        }

        public static ICsvProcessable DeserializeFromCsvRow(string row)
        {
            string[] splits = row.Split(',');
            return new Manufacturer(ServicesHelper.GetService<DataCacherService<Country>>().GetFirstBy(n => n.Code == splits[1]).ID, splits[0]);
        }

        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as Manufacturer);
        }

        public bool Equals(Manufacturer? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Manufacturer? left, Manufacturer? right)
        {
            return EqualityComparer<Manufacturer>.Default.Equals(left, right);
        }

        public static bool operator !=(Manufacturer? left, Manufacturer? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
