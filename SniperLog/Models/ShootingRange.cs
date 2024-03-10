using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using SniperLog.Services.Database;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public class ShootingRange : IDataAccessObject
    {
        #region Properties
        public int ID { get; set; }

        public string Name { get; set; }

        public string? Address { get; set; }

        private double _latitude;
        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;
                Location = new Location(_latitude, _longitude);
            }
        }

        private double _longitude;
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
                Location = new Location(_latitude, _longitude);
            }
        }

        public string? RelativeImagePathFromAppdata { get; set; }

        [DatabaseIgnore]
        public Location Location { get; set; }

        public static string SelectAllQuery => DataAccessObjectQueryBuilder.GetSelectQuery<ShootingRange>();

        public static string InsertQuery => DataAccessObjectQueryBuilder.GetInsertQuery<ShootingRange>(insertOrUpdate: true, includeReturning: true);

        public static string InsertQueryNoId => DataAccessObjectQueryBuilder.GetInsertQuery<ShootingRange>(insertOrUpdate: true, includeReturning: true);

        public static string DeleteQuery => DataAccessObjectQueryBuilder.GetDeleteQuery<ShootingRange>();

        #endregion

        #region Constructors
        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="name"></param>s
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="relativeImagePathFromAppdata"></param>
        public ShootingRange(int iD, string name, string? address, double latitude, double longitude, string? relativeImagePathFromAppdata)
        {
            ID = iD;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            RelativeImagePathFromAppdata = relativeImagePathFromAppdata;
        }

        /// <summary>
        /// New object constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="relativeImagePathFromAppdata"></param>
        public ShootingRange(string name, string? address, double latitude, double longitude, string? relativeImagePathFromAppdata) : this(-1, name, address, latitude, longitude, relativeImagePathFromAppdata) { }
        #endregion

        #region DAO Methods
        public async Task<int> SaveAsync()
        {
            if (ID == -1)
            {
                ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId,
                    new SqliteParameter("@Name", Name),
                    new SqliteParameter("@Address", Address),
                    new SqliteParameter("@Longitude", Longitude),
                    new SqliteParameter("@Latitude", Latitude),
                    new SqliteParameter("@RelativeImagePathFromAppdata", RelativeImagePathFromAppdata));
                return ID;
            }

            return await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQuery,
                new SqliteParameter("@ID", ID),
                new SqliteParameter("@Name", Name),
                new SqliteParameter("@Address", Address),
                new SqliteParameter("@Longitude", Longitude),
                new SqliteParameter("@Latitude", Latitude),
                new SqliteParameter("@RelativeImagePathFromAppdata", RelativeImagePathFromAppdata));
        }

        public async Task<bool> DeleteAsync()
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new ShootingRange(
                row.GetConverted<int>("ID"),
                row.GetConverted<string?>("Name"),
                row.GetConverted<string>("Address"),
                row.GetConverted<double>("Latitude"),
                row.GetConverted<double>("Longitude"),
                row.GetConverted<string>("RelativeImagePathFromAppdata")
                );
        }

        #endregion

        #region Model Specific Methods
        public bool TryGetBackgroundImage(out string path)
        {
            string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRanges", Name));
            foreach (string file in Directory.GetFiles(dataPath))
            {
                if (Path.GetFileNameWithoutExtension(file).Contains("BackgroundImage"))
                {
                    path = file;
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }

        public async Task SaveBackgroundImageAsync(string originalFilePathFull)
        {
            if (!File.Exists(originalFilePathFull))
            {
                throw new ArgumentException("Background image file cannot be null");
            }

            if (!FileExtensionsChecker.IsImageFile(originalFilePathFull))
            {
                throw new ArgumentException("File is not an image file");
            }

            string extension = Path.GetExtension(originalFilePathFull);
            byte[] data = await File.ReadAllBytesAsync(originalFilePathFull);

            string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRanges", Name, $"BackgroundImage{extension}"));

            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
            await File.WriteAllBytesAsync(dataPath, data);

            await Shell.Current.DisplayAlert("Debug", dataPath, "Okay");
        }

        public string GetBackgroundImagePath(string extension)
        {
            return AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRanges", Name, $"BackgroundImage{extension}"));
        }
        #endregion
    }
}