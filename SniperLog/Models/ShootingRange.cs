using Microsoft.Data.Sqlite;
using SniperLog.Services;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Models
{
    public class ShootingRange : IDataAccessObject<ShootingRange>, IEquatable<ShootingRange?>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? RelativeImagePathFromAppdata { get; set; }

        public ShootingRange(int iD, string name, string? address, double latitude, double longitude, string? relativeImagePathFromAppdata)
        {
            ID = iD;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            RelativeImagePathFromAppdata = relativeImagePathFromAppdata;
        }

        public ShootingRange(string name, string? address, double latitude, double longitude, string? relativeImagePathFromAppdata)
        {
            ID = -1;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            RelativeImagePathFromAppdata = relativeImagePathFromAppdata;
        }

        public static async Task<ShootingRange?> LoadAsync(int id)
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
            return LoadNextFromRow(table.Rows[0]);
        }

        /// <summary>
        /// Saves or replaces the instance with newer data and returns instance ID in database
        /// </summary>
        /// <returns>ID if the instance</returns>
        public async Task<int> SaveAsync()
        {
            if (ID == -1) return await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync("INSERT INTO ShootingRange(Name, Address, Latitude, Longitude) VALUES(@Name, @Address, @Latitude, @Longitude) RETURNING ShootingRange.ID", new SqliteParameter("@Name", Name), new SqliteParameter("@Address", Address), new SqliteParameter("@Latitude", Latitude), new SqliteParameter("@Longitude", Longitude));
            else return await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync("INSERT OR REPLACE INTO ShootingRange(ID, Name, Address, Latitude, Longitude) VALUES(SELECT ShootingRange.ID FROM ShootingRange WHERE ShootingRange.Name = @Name), @Name, @Address, @Latitude, @Longitude)", new SqliteParameter("@Name", Name), new SqliteParameter("@Address", Address), new SqliteParameter("@Latitude", Latitude), new SqliteParameter("@Longitude", Longitude));
        }

        public async Task<bool> DeleteAsync()
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync("DELETE FROM ShootingRange WHERE ShootingRange.ID = @ID", new SqliteParameter("@ID", ID)) == 1;
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
                collection.Add(LoadNextFromRow(row));
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
                collection.Add(LoadNextFromRow(row));
            }

            return collection;
        }


        public static ShootingRange LoadNextFromRow(DataRow reader)
        {
            return new ShootingRange(Convert.ToInt32(reader["ID"]), Convert.ToString(reader["Name"]), Convert.ToString(reader["Address"]), Convert.ToDouble(reader["Latitude"]), Convert.ToDouble(reader["Longitude"]), Convert.ToString(reader["RelativeImagePathFromAppdata"]));
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