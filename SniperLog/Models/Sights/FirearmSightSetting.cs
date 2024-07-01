using Microsoft.Data.Sqlite;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class FirearmSightSetting : IDataAccessObject, ICsvProcessable, IEquatable<FirearmSightSetting?>
    {

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(FirearmSight), nameof(FirearmSight.ID))]
        public int FirearmSight_ID { get; set; }

        public int Distance { get; set; }

        public int ElevationValue { get; set; }

        public int WindageValue { get; set; }

        public string DistanceAsText => Distance + "m";
        public string ElevationAsText => ElevationValue + " up";
        public string WindageAsText => WindageValue + " right";

        public static string CsvHeader => "FirearmSightName,Distance,Elevation,Windage";

        #endregion

        #region CSV

        public static ICsvProcessable DeserializeFromCsvRow(string row)
        {
            string[] split = row.Split(',');
            FirearmSight sight = ServicesHelper.GetService<DataCacherService<FirearmSight>>().GetFirstBy(n => n.Name == split[0]);
            return new FirearmSightSetting(sight.ID, int.Parse(split[2]), int.Parse(split[2]), int.Parse(split[2]), int.Parse(split[2]));
        }

        public string SerializeToCsvRow()
        {
            return string.Join(',', ReferencedFirearmSight.Name, Distance, ElevationValue, WindageValue);
        }

        #endregion

        #region Constructor

        public FirearmSightSetting(int iD, int firearmSight_ID, int distance, int elevationValue, int windageValue)
        {
            ID = iD;
            FirearmSight_ID = firearmSight_ID;
            Distance = distance;
            ElevationValue = elevationValue;
            WindageValue = windageValue;
        }

        public FirearmSightSetting(int firearmSight_ID, int distance, int elevationValue, int windageValue) : this(-1, firearmSight_ID, distance, elevationValue, windageValue) { }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new FirearmSightSetting(row);
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
                await ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>().AddOrUpdateAsync(this);
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
                await ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>().RemoveAsync(this);
            }
        }

        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as FirearmSightSetting);
        }

        public bool Equals(FirearmSightSetting? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FirearmSightSetting? left, FirearmSightSetting? right)
        {
            return EqualityComparer<FirearmSightSetting>.Default.Equals(left, right);
        }

        public static bool operator !=(FirearmSightSetting? left, FirearmSightSetting? right)
        {
            return !(left == right);
        }

        #endregion
    }
}