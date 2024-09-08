using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using System.Data;

namespace SniperLog.Models
{
    /// <summary>
    /// A class representing subrange of a shooting range.<br></br>
    /// Notes are saved in a txt file saved in Data/ShootingRanges/RangeName/SubRanges/Notes%ID%.txt
    /// </summary>
    public partial class SubRange : ObservableObject, IDataAccessObject, INoteSaveable, IEquatable<SubRange?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(ShootingRange), nameof(ShootingRange.ID))]
        [ObservableProperty]
        public int _shootingRange_ID;

        [ObservableProperty]
        public int _rangeInMeters;

        [ObservableProperty]
        public double? _altitude;

        [ObservableProperty]
        public int? _directionToNorthDegrees;

        [ObservableProperty]
        public int? _verticalFiringOffsetDegrees;

        [ObservableProperty]
        private char _prefix;

        public string NotesSavePath
        {
            get
            {
                return Path.Combine("Data", "ShootingRanges", ReferencedShootingRange.Name, "SubRanges", $"Notes{ID}.txt");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary ctor
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="shootingRange_ID"></param>
        /// <param name="rangeInMeters"></param>
        /// <param name="altitude"></param>
        /// <param name="directionToNorth"></param>
        /// <param name="verticalFiringOffsetDegrees"></param>
        public SubRange(int iD, int shootingRange_ID, int rangeInMeters, double? altitude, int? directionToNorth, int? verticalFiringOffsetDegrees, char displayLetter)
        {
            ID = iD;
            ShootingRange_ID = shootingRange_ID;
            RangeInMeters = rangeInMeters;
            Altitude = altitude;
            DirectionToNorthDegrees = directionToNorth;
            VerticalFiringOffsetDegrees = verticalFiringOffsetDegrees;
            Prefix = displayLetter;
        }

        /// <summary>
        /// Idless ctor
        /// </summary>
        /// <param name="shootingRange_ID"></param>
        /// <param name="rangeInMeters"></param>
        /// <param name="altitude"></param>
        /// <param name="directionToNorth"></param>
        /// <param name="verticalFiringOffsetDegrees"></param>
        public SubRange(int shootingRange_ID, int rangeInMeters, double? altitude, int? directionToNorth, int? verticalFiringOffsetDegrees, char displayLetter) : this(-1, shootingRange_ID, rangeInMeters, altitude, directionToNorth, verticalFiringOffsetDegrees, displayLetter)
        {

        }

        #endregion

        #region DAO Methods

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
                ServicesHelper.GetService<DataCacherService<SubRange>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<SubRange>>().Remove(this);
                DeleteNotes();
            }
        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new SubRange(row);
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return Prefix.ToString();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SubRange);
        }

        public bool Equals(SubRange? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SubRange? left, SubRange? right)
        {
            return EqualityComparer<SubRange>.Default.Equals(left, right);
        }

        public static bool operator !=(SubRange? left, SubRange? right)
        {
            return !(left == right);
        }

        #endregion

    }
}
