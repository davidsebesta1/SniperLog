using Microsoft.Data.Sqlite;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class FirearmSight : ObservableObject, IDataAccessObject, ICsvProcessable, IEquatable<FirearmSight?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(SightClickType), nameof(SightClickType.ID))]
        public int ClickType_ID { get; set; }

        [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
        public int Manufacturer_ID { get; set; }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private double _oneClickValue;

        public static string CsvHeader => "Name,ClickType,ManufacturerName,OneClickValue";

        #endregion

        #region Constructors

        public FirearmSight(int iD, int clickType_ID, int manufacturer_ID, string name, double oneClickValue)
        {
            ID = iD;
            ClickType_ID = clickType_ID;
            Manufacturer_ID = manufacturer_ID;
            Name = name;
            OneClickValue = oneClickValue;
        }

        public FirearmSight(int clickType_ID, int manufacturer_ID, string name, double oneClickValue) : this(-1, clickType_ID, manufacturer_ID, name, oneClickValue) { }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new FirearmSight(row);
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
                await ServicesHelper.GetService<DataCacherService<FirearmSight>>().AddOrUpdateAsync(this);
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
                await ServicesHelper.GetService<DataCacherService<FirearmSight>>().RemoveAsync(this);
            }
        }
        #endregion

        #region CSV

        public static ICsvProcessable DeserializeFromCsvRow(string row)
        {
            string[] split = row.Split(',');
            SightClickType sightClickType = ServicesHelper.GetService<DataCacherService<SightClickType>>().GetFirstBy(n => n.ClickTypeName == split[1]);
            Manufacturer manufacturer = ServicesHelper.GetService<DataCacherService<Manufacturer>>().GetFirstBy(n => n.Name == split[2]);
            return new FirearmSight(sightClickType.ID, manufacturer.ID, split[0], double.Parse(split[3]));
        }

        public string SerializeToCsvRow()
        {
            return string.Join(',', Name, ReferencedSightClickType.ClickTypeName, ReferencedManufacturer.Name, _oneClickValue);
        }

        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as FirearmSight);
        }

        public bool Equals(FirearmSight? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FirearmSight? left, FirearmSight? right)
        {
            return EqualityComparer<FirearmSight>.Default.Equals(left, right);
        }

        public static bool operator !=(FirearmSight? left, FirearmSight? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
