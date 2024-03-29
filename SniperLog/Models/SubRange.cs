﻿using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using SniperLog.Services;
using SniperLog.Services.Database;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class SubRange : IDataAccessObject
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(ShootingRange), nameof(ShootingRange.ID))]
        public int ShootingRange_ID { get; set; }

        public ShootingRange? ReferencedShootingRange
        {
            get
            {
                return ServicesHelper.GetService<DataCacherService<ShootingRange>>().GetFirstBy(n => n.ID == ShootingRange_ID);
            }
        }

        public int RangeInMeters { get; set; }

        public double? Altitude { get; set; }

        public double? DirectionToNorth { get; set; }

        public double? VerticalFiringOffsetDegrees { get; set; }

        public string? NotesRelativePathFromAppData { get; set; }

        public static string SelectAllQuery => DataAccessObjectQueryBuilder.GetSelectQuery<SubRange>();
        public static string InsertQuery => DataAccessObjectQueryBuilder.GetInsertQuery<SubRange>(includePK: true, insertOrUpdate: true, includeReturning: true);
        public static string InsertQueryNoId => DataAccessObjectQueryBuilder.GetInsertQuery<SubRange>(insertOrUpdate: true, includeReturning: false);
        public static string DeleteQuery => DataAccessObjectQueryBuilder.GetDeleteQuery<SubRange>();

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
        public SubRange(int iD, int shootingRange_ID, int rangeInMeters, double? altitude, double? directionToNorth, double? verticalFiringOffsetDegrees, string? notesRelativePathFromAppData)
        {
            ID = iD;
            ShootingRange_ID = shootingRange_ID;
            RangeInMeters = rangeInMeters;
            Altitude = altitude;
            DirectionToNorth = directionToNorth;
            VerticalFiringOffsetDegrees = verticalFiringOffsetDegrees;
            NotesRelativePathFromAppData = notesRelativePathFromAppData;
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
        public SubRange(int shootingRange_ID, int rangeInMeters, double? altitude, double? directionToNorth, double? verticalFiringOffsetDegrees, string? notesRelativePathFromAppData) : this(-1, shootingRange_ID, rangeInMeters, altitude, directionToNorth, verticalFiringOffsetDegrees, notesRelativePathFromAppData) { }

        #endregion

        #region DAO Methods

        public async Task<int> SaveAsync()
        {
            if (ID == -1)
            {
                ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId,
                    new SqliteParameter("@ShootingRange_ID", ShootingRange_ID),
                    new SqliteParameter("@RangeInMeters", RangeInMeters),
                    new SqliteParameter("@Altitude", Altitude),
                    new SqliteParameter("@DirectionToNorth", DirectionToNorth),
                    new SqliteParameter("@VerticalFiringOffsetDegrees", VerticalFiringOffsetDegrees),
                    new SqliteParameter("@NotesRelativePathFromAppData", NotesRelativePathFromAppData));
                return ID;
            }
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery,
                new SqliteParameter("@ID", ID),
                new SqliteParameter("@ShootingRange_ID", ShootingRange_ID),
                new SqliteParameter("@RangeInMeters", RangeInMeters),
                new SqliteParameter("@Altitude", Altitude),
                new SqliteParameter("@DirectionToNorth", DirectionToNorth),
                new SqliteParameter("@VerticalFiringOffsetDegrees", VerticalFiringOffsetDegrees),
                new SqliteParameter("@NotesRelativePathFromAppData", NotesRelativePathFromAppData));

        }
        public async Task<bool> DeleteAsync()
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new SubRange(
                row.GetConverted<int>("ID"),
                row.GetConverted<int>("ShootingRange_ID"),
                row.GetConverted<int>("RangeInMeters"),
                row.GetConverted<double?>("Altitude"),
                row.GetConverted<double?>("DirectionToNorth"),
                row.GetConverted<double>("VerticalFiringOffsetDegrees"),
                row.GetConverted<string>("NotesRelativePathFromAppData")
                );
        }

        #endregion

        #region Object specific method

        public async Task SaveNotes(string newNotes)
        {
            string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRanges", $"SubRanges{ID}", $"notes.txt"));

            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
            NotesRelativePathFromAppData = dataPath;
            await File.WriteAllTextAsync(dataPath, newNotes);
            await SaveAsync();
        }

        #endregion

    }
}
