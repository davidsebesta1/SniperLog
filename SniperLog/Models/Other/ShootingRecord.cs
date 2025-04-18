﻿using Microsoft.Data.Sqlite;
using SniperLog.Extensions.WrapperClasses;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Models
{
    public partial class ShootingRecord : ObservableObject, IDataAccessObject, INoteSaveable, IEquatable<ShootingRecord?>
    {
        public string NotesSavePath
        {
            get
            {
                return Path.Combine("Data", "ShootingRecords", ReferencedFirearm.Name, $"{ID}.txt");
            }
        }

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        [ForeignKey(typeof(ShootingRange), nameof(ShootingRange.ID))]
        private int _shootingRange_ID;

        [ObservableProperty]
        [ForeignKey(typeof(SubRange), nameof(SubRange.ID))]
        private int _subRange_ID;

        [ObservableProperty]
        [ForeignKey(typeof(Firearm), nameof(Firearm.ID))]
        private int _firearm_ID;

        [ObservableProperty]
        [ForeignKey(typeof(Ammunition), nameof(Ammunition.ID))]
        private int _ammo_ID;

        [ObservableProperty]
        [ForeignKey(typeof(Weather), nameof(Weather.ID))]
        private int? _weather_ID;

        /// <summary>
        /// Total clicks done from 0.
        /// </summary>
        [ObservableProperty]
        private int _elevationClicksOffset;

        /// <summary>
        /// Total clicks done from 0.
        /// </summary>
        [ObservableProperty]
        private int _windageClicksOffset;

        /// <summary>
        /// Distance at which this record was taken.
        /// </summary>
        [ObservableProperty]
        private int _distance;

        /// <summary>
        /// Bitwise representation of time for serialization. Use <see cref="Date"/> for actual date.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Date))]
        private long _timeTaken;

        /// <summary>
        /// Date and time at which this record was taken.
        /// </summary>
        public DateTime Date => DateTime.FromBinary(TimeTaken);

        #endregion

        #region Ctor

        /// <summary>
        /// Main ctor.
        /// </summary>
        public ShootingRecord(int iD, int srange_ID, int subrange_ID, int firearm_ID, int ammo_ID, int? weather_ID, int elevationClicksOffset, int windageClicksOffset, int distance, long timeTaken)
        {
            ID = iD;
            ShootingRange_ID = srange_ID;
            SubRange_ID = subrange_ID;
            Firearm_ID = firearm_ID;
            Ammo_ID = ammo_ID;
            Weather_ID = weather_ID;
            ElevationClicksOffset = elevationClicksOffset;
            WindageClicksOffset = windageClicksOffset;
            Distance = distance;
            TimeTaken = timeTaken;
        }

        /// <summary>
        /// ID-less ctor.
        /// </summary>
        public ShootingRecord(int srange_ID, int subrange_ID, int firearm_ID, int ammo_ID, int? weather_ID, int elevationClicksOffset, int windageClicksOffset, int distance, long timeTaken) : this(-1, srange_ID, subrange_ID, firearm_ID, ammo_ID, weather_ID, elevationClicksOffset, windageClicksOffset, distance, timeTaken) { }

        #endregion

        #region DAO

        /// <inheritdoc/>
        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new ShootingRecord(row);
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
                ServicesHelper.GetService<DataCacherService<ShootingRecord>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<ShootingRecord>>().Remove(this);
            }
        }

        #endregion

        #region Model specific

        /// <summary>
        /// Saves an image object with reference to this record.
        /// </summary>
        /// <param name="paths">Path to the image.</param>
        public async Task SaveImageAsync(DrawableImagePaths paths)
        {
            ShootingRecordImage recordImage = new ShootingRecordImage(ID);
            await recordImage.SaveAsync();
            await recordImage.SaveImageAsync(paths);
        }

        public async Task<ObservableCollection<ShootingRecordImage>> GetImagesAsync()
        {
            return await ServicesHelper.GetService<DataCacherService<ShootingRecordImage>>().GetAllBy(n => n.ShootingRecord_ID == ID);
        }

        #endregion

        #region Object

        public override bool Equals(object? obj)
        {
            return Equals(obj as ShootingRecord);
        }

        public bool Equals(ShootingRecord? other)
        {
            return other is not null && ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(ShootingRecord? left, ShootingRecord? right)
        {
            return EqualityComparer<ShootingRecord>.Default.Equals(left, right);
        }

        public static bool operator !=(ShootingRecord? left, ShootingRecord? right)
        {
            return !(left == right);
        }

        #endregion
    }
}