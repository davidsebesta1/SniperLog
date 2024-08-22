using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class Manufacturer : ObservableObject, IDataAccessObject, ICsvProcessable, IEquatable<Manufacturer?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(Country), nameof(Country.ID))]
        public int Country_ID { get; set; }

        [ForeignKey(typeof(ManufacturerType), nameof(ManufacturerType.ID))]
        public int ManufacturerType_ID { get; set; }

        [ObservableProperty]
        private string _name;

        public static string CsvHeader => "Name,CountryCode";

        #endregion

        #region Constructors

        public Manufacturer(int iD, int country_ID, int manuType_ID, string name)
        {
            ID = iD;
            Country_ID = country_ID;
            ManufacturerType_ID = manuType_ID;
            Name = name;
        }

        public Manufacturer(int country_ID, int manuType_ID, string name) : this(-1, country_ID, manuType_ID, name)
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
                ServicesHelper.GetService<DataCacherService<Manufacturer>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<Manufacturer>>().Remove(this);
            }
        }

        #endregion

        #region CSV

        public string SerializeToCsvRow()
        {
            return string.Join(',', Name, ReferencedManufacturerType.Name, ReferencedCountry.Code);
        }

        public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
        {
            string[] splits = row.Split(',');
            return new Manufacturer((await ServicesHelper.GetService<DataCacherService<Country>>().GetFirstBy(n => n.Code == splits[1])).ID, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(n => n.Name == splits[2])).ID, splits[0]);
        }

        #endregion

        #region Object methods

        public override string ToString()
        {
            return Name;
        }

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
