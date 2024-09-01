using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class ShootingRecord : ObservableObject, IDataAccessObject, INoteSaveable
    {
        public string NotesSavePath
        {
            get
            {
                return Path.Combine("Data", "ShootingRecords", ReferencedFirearm.Name, $"{ID}.txt");
            }
        }

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        [ForeignKey(typeof(ShootingRange), nameof(ShootingRange.ID))]
        private int _shootingRange_ID;

        [ObservableProperty]
        [ForeignKey(typeof(SubRange), nameof(SubRange.ID))]
        public int _subRange_ID;

        [ObservableProperty]
        [ForeignKey(typeof(Firearm), nameof(Firearm.ID))]
        private int _firearm_ID;

        [ObservableProperty]
        private int _elevationClicksOffset;

        [ObservableProperty]
        private int _windageClicksOffset;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Date))]
        private long _timeTaken;

        public DateTime Date => DateTime.FromBinary(TimeTaken);

        public ShootingRecord(int iD, int srange_ID, int subrange_ID, int firearm_ID, int elevationClicksOffset, int windageClicksOffset, long timeTaken)
        {
            ID = iD;
            ShootingRange_ID = srange_ID;
            SubRange_ID = subrange_ID;
            Firearm_ID = firearm_ID;
            ElevationClicksOffset = elevationClicksOffset;
            WindageClicksOffset = windageClicksOffset;
            TimeTaken = timeTaken;
        }

        public ShootingRecord(int srange_ID, int subrange_ID, int firearm_ID, int elevationClicksOffset, int windageClicksOffset, long timeTaken) : this(-1, srange_ID, subrange_ID, firearm_ID, elevationClicksOffset, windageClicksOffset, timeTaken) { }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new ShootingRecord(row);
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
                ServicesHelper.GetService<DataCacherService<ShootingRecord>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<ShootingRecord>>().Remove(this);
            }
        }
    }
}