using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class FirearmSightSetting : ObservableObject, IDataAccessObject, ICsvProcessable, IEquatable<FirearmSightSetting?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(FirearmSight), nameof(FirearmSight.ID))]
        public int FirearmSight_ID { get; set; }

        [ObservableProperty]
        private int _distance;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DeltaElevation))]
        [NotifyPropertyChangedFor(nameof(DeltaWindage))]
        private int _elevationValue;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DeltaElevation))]
        [NotifyPropertyChangedFor(nameof(DeltaWindage))]
        private int _windageValue;

        [ObservableProperty]
        [DatabaseIgnore]
        [NotifyPropertyChangedFor(nameof(DeltaElevation))]
        [NotifyPropertyChangedFor(nameof(DeltaWindage))]
        private FirearmSightSetting? _previousSetting = null;

        [DatabaseIgnore]
        public string? DeltaElevation
        {
            get
            {
                if (PreviousSetting == null)
                {
                    return null;
                }

                int delta = ElevationValue - PreviousSetting.ElevationValue;
                if (delta == 0)
                {
                    return null;
                }

                return delta.ToString("+0;-#");
            }
        }

        [DatabaseIgnore]
        public string? DeltaWindage
        {
            get
            {
                if (PreviousSetting == null)
                {
                    return null;
                }

                int delta = WindageValue - PreviousSetting.WindageValue;
                if (delta == 0)
                {
                    return null;
                }

                return delta.ToString("+0;-#");

            }
        }

        public static string CsvHeader => "FirearmSightName,Distance,Elevation,Windage";

        #endregion

        #region CSV

        public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
        {
            string[] split = row.Split(',');
            FirearmSight sight = await ServicesHelper.GetService<DataCacherService<FirearmSight>>().GetFirstBy(n => n.Name == split[0]);
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
                ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>().Remove(this);
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