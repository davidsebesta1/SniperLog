using Microsoft.Data.Sqlite;
using SniperLog.Models.Interfaces;
using SniperLog.Services.Database;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class SubRange : IDataAccessObject, IEquatable<SubRange?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        public bool IsDefault { get; set; }

        [ForeignKey(typeof(ShootingRange), nameof(ShootingRange.ID))]
        public int ShootingRange_ID { get; set; }

        public int RangeInMeters { get; set; }

        public double? Altitude { get; set; }

        public double? DirectionToNorth { get; set; }

        public double? VerticalFiringOffsetDegrees { get; set; }

        [DatabaseIgnore]
        public char DisplayLetter
        {
            get
            {
                var subRanges = ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ShootingRange_ID);
                int index = subRanges.IndexOf(this);

                char letter = (char)(65 + index);

                return letter;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="shootingRange_ID"></param>
        /// <param name="rangeInMeters"></param>
        /// <param name="altitude"></param>
        /// <param name="directionToNorth"></param>
        /// <param name="verticalFiringOffsetDegrees"></param>
        /// <param name="notesRelativePathFromAppData"></param>
        public SubRange(int iD, bool isDefault, int shootingRange_ID, int rangeInMeters, double? altitude, double? directionToNorth, double? verticalFiringOffsetDegrees)
        {
            ID = iD;
            IsDefault = isDefault;
            ShootingRange_ID = shootingRange_ID;
            RangeInMeters = rangeInMeters;
            Altitude = altitude;
            DirectionToNorth = directionToNorth;
            VerticalFiringOffsetDegrees = verticalFiringOffsetDegrees;
        }

        /// <summary>
        /// ID-less constructor
        /// </summary>
        /// <param name="shootingRange_ID"></param>
        /// <param name="rangeInMeters"></param>
        /// <param name="altitude"></param>
        /// <param name="directionToNorth"></param>
        /// <param name="verticalFiringOffsetDegrees"></param>
        /// <param name="notesRelativePathFromAppData"></param>
        public SubRange(int shootingRange_ID, bool isDefault, int rangeInMeters, double? altitude, double? directionToNorth, double? verticalFiringOffsetDegrees) : this(-1, isDefault, shootingRange_ID, rangeInMeters, altitude, directionToNorth, verticalFiringOffsetDegrees)
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
                await ServicesHelper.GetService<DataCacherService<SubRange>>().AddOrUpdateAsync(this);
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
                await ServicesHelper.GetService<DataCacherService<SubRange>>().RemoveAsync(this);
            }
        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new SubRange(row);
        }

        #endregion

        #region Equals

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
