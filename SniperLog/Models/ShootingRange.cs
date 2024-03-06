using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using SniperLog.Services;
using SniperLog.Services.Database;
using SniperLog.Services.Database.Attributes;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Models
{
    public class ShootingRange : IDataAccessObject<ShootingRange>, IEquatable<ShootingRange?>
    {
        [PrimaryKey]
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

        private static readonly string InsertQuery = DataAccessObjectInsertQueryBuilder.GetInsertQueryStatement<ShootingRange>(addReturningId: true, insertOrUpdate: true);
        private static readonly string InsertQueryNoId = DataAccessObjectInsertQueryBuilder.GetInsertQueryStatement<ShootingRange>(addReturningId: false, insertOrUpdate: true);

        [DatabaseIgnore]
        public Location Location { get; set; }

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

        public ShootingRange(string name, string? address, double latitude, double longitude, string? relativeImagePathFromAppdata) : this(-1, name, address, latitude, longitude, relativeImagePathFromAppdata) { }

        public static async Task<ShootingRange?> LoadNewAsync(int id)
        {
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM ShootingRange WHERE ShootingRange.ID = @ID", new SqliteParameter("@ID", id));

            if (table == null)
            {
                return null;
            }

            if (table.Rows.Count > 1 || table.Rows.Count == 0)
            {
                throw new Exception($"Retrieved multiple instances from database of {nameof(ShootingRange)}, expected: 1 or 0");
            }
            return LoadFromRow(table.Rows[0]);
        }

        /// <summary>
        /// Saves or replaces the instance with newer data and returns instance ID in database
        /// </summary>
        /// <returns>ID if the instance</returns>
        public async Task<int> SaveAsync()
        {
            if (ID == -1) return ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQuery, new SqliteParameter("@Name", Name), new SqliteParameter("@Address", Address), new SqliteParameter("@Latitude", Latitude), new SqliteParameter("@Longitude", Longitude), new SqliteParameter("@RelativeImagePathFromAppdata", RelativeImagePathFromAppdata));
            else return await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId, new SqliteParameter("@Name", Name), new SqliteParameter("@Address", Address), new SqliteParameter("@Latitude", Latitude), new SqliteParameter("@Longitude", Longitude), new SqliteParameter("@RelativeImagePathFromAppdata", RelativeImagePathFromAppdata));
        }

        public async Task<bool> DeleteAsync()
        {
            bool result = await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync("DELETE FROM ShootingRange WHERE ShootingRange.ID = @ID", new SqliteParameter("@ID", ID)) == 1;

            try
            {
                if (result)
                {
                    AppDataFileHelper.DeleteFolderFromLocation(Path.GetDirectoryName(RelativeImagePathFromAppdata));
                }
            }
            catch (Exception ex)
            {
                //add log
            }

            return result;
        }

        public static async Task<ObservableCollection<ShootingRange>> LoadAllAsync()
        {
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM ShootingRange");

            if (table == null)
            {
                throw new NullReferenceException("Table is null");
            }

            ObservableCollection<ShootingRange> collection = new ObservableCollection<ShootingRange>();
            foreach (DataRow row in table.Rows)
            {
                collection.Add(LoadFromRow(row));
            }

            return collection;
        }

        public static async Task<ObservableCollection<ShootingRange>> LoadAllAsync(ObservableCollection<ShootingRange> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Passed observable collection cannot be null");
            }

            collection.Clear();
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM ShootingRange");

            if (table == null)
            {
                throw new NullReferenceException("Table is null");
            }

            foreach (DataRow row in table.Rows)
            {
                collection.Add(LoadFromRow(row));
            }

            return collection;
        }

        public static ShootingRange LoadFromRow(DataRow reader)
        {
            return new ShootingRange(Convert.ToInt32(reader["ID"]), Convert.ToString(reader["Name"]), Convert.ToString(reader["Address"]), Convert.ToDouble(reader["Latitude"]), Convert.ToDouble(reader["Longitude"]), Convert.ToString(reader["RelativeImagePathFromAppdata"]));
        }

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

        public static ShootingRange? GetById(int id)
        {
            DataService<ShootingRange> service = Application.Current.MainPage.Handler.MauiContext.Services.GetService<DataService<ShootingRange>>();
            ShootingRange? range = service.GetFirstBy(n => n.ID == id);

            return range;
        }

        public string GetBackgroundImagePath(string extension)
        {
            return AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRanges", Name, $"BackgroundImage{extension}"));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ShootingRange);
        }

        public bool Equals(ShootingRange? other)
        {
            return other is not null && ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Address, Latitude, Longitude);
        }


        public static bool operator ==(ShootingRange? left, ShootingRange? right)
        {
            return EqualityComparer<ShootingRange>.Default.Equals(left, right);
        }

        public static bool operator !=(ShootingRange? left, ShootingRange? right)
        {
            return !(left == right);
        }
    }
}