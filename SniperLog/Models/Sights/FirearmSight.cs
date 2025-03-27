using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using SniperLog.Services.Database.Attributes;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Models
{
    public partial class FirearmSight : ObservableObject, IDataAccessObject, IEquatable<FirearmSight?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(SightClickType), nameof(SightClickType.ID))]
        [ObservableProperty]
        private int _clickType_ID;

        [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
        [ObservableProperty]
        private int _manufacturer_ID;

        [ForeignKey(typeof(SightReticle), nameof(SightReticle.ID))]
        [ObservableProperty]
        private int _sightReticle_ID;

        [ObservableProperty]
        private string _name;

        /// <summary>
        /// Value of a single click. Dependant on <see cref="ReferencedSightClickType"/>.
        /// </summary>
        [ObservableProperty]
        private double _oneClickValue;

        public static string CsvHeader => "Name,ClickType,ManufacturerName,OneClickValue";

        #endregion

        #region Constructors

        public FirearmSight(int iD, int clickType_ID, int manufacturer_ID, int sightReticle_ID, string name, double oneClickValue)
        {
            ID = iD;
            ClickType_ID = clickType_ID;
            Manufacturer_ID = manufacturer_ID;
            SightReticle_ID = sightReticle_ID;
            Name = name;
            OneClickValue = oneClickValue;
        }

        public FirearmSight(int clickType_ID, int manufacturer_ID, int sightReticle_ID, string name, double oneClickValue) : this(-1, clickType_ID, manufacturer_ID, sightReticle_ID, name, oneClickValue)
        {

        }

        #endregion

        #region DAO Methods

        /// <inheritdoc/>
        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new FirearmSight(row);
        }

        /// <inheritdoc/>
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
                ServicesHelper.GetService<DataCacherService<FirearmSight>>().AddOrUpdate(this);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<FirearmSight>>().Remove(this);
            }
        }

        /// <summary>
        /// Gets an unsorted collection of <see cref="FirearmSightSetting"/> which have relation to this sight.
        /// </summary>
        /// <returns>A new collection or null if this object hasn't been saved yet.</returns>
        public async Task<ObservableCollection<FirearmSightSetting>> GetBaseSightSettingsAsync()
        {
            if (ID == -1)
                return null;

            DataCacherService<FirearmSightSetting> cacher = ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>();
            return await cacher.GetAllBy(n => n.FirearmSight_ID == ID);
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FirearmSight);
        }

        public bool Equals(FirearmSight? other)
        {
            return other is not null && ID == other.ID;
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

    /// <summary>
    /// Struct for click offsets.
    /// </summary>
    public readonly struct ClickOffset
    {
        /// <summary>
        /// Vertical clicks offset..
        /// </summary>
        public readonly int? VerticalClicks;

        /// <summary>
        /// Horizontal clicks offset.
        /// </summary>
        public readonly int? WindageClicks;

        /// <summary>
        /// Distance for which this click offset is.
        /// </summary>
        public readonly int? Distance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verticalClicks">Vertical clicks.</param>
        /// <param name="horizontalClicks">Horizontal clicks.</param>
        /// <param name="distance">Distance.</param>
        public ClickOffset(int? verticalClicks, int? horizontalClicks, int? distance)
        {
            VerticalClicks = verticalClicks;
            WindageClicks = horizontalClicks;
            Distance = distance;
        }
    }
}
