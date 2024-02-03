using Microsoft.Data.Sqlite;
using SniperLog.Services;
using System.Collections.ObjectModel;

namespace SniperLog.Models
{
    public class ShootingRange : IDataAccessObject<ShootingRange>
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static async Task<ShootingRange?> LoadAsync(int id)
        {
            SqliteDataReader? reader = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM ShootingRange WHERE ShootingRange.ID = @ID", new SqliteParameter("@ID", id));

            if (reader == null)
            {
                return null;
            }

            int reads = 0;
            while (await reader.ReadAsync())
            {
                reads++;

                if (reads > 1)
                {
                    throw new Exception($"There are multiple {nameof(ShootingRange)} returned rows with same ID");
                }
            }

            return LoadNextFromReader(reader);
        }

        public async Task<bool> SaveAsync()
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync("INSERT OR REPLACE INTO ShootingRange(ID, Name Address, Latitude, Longitude) VALUES(SELECT ShootingRange.ID FROM ShootingRange WHERE ShootingRange.Name = @Name), @Name, @Address, @Latitude, @Longitude)", new SqliteParameter("@Name", Name), new SqliteParameter("@Address", Address), new SqliteParameter("@Latitude", Latitude), new SqliteParameter("@Longitude", Longitude)) == 1;
        }

        public async Task<bool> DeleteAsync()
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync("DELETE FROM ShootingRange WHERE ShootingRange.ID = @ID", new SqliteParameter("@ID", ID)) == 1;
        }

        public static async Task<ObservableCollection<ShootingRange>> LoadAllAsync()
        {
            SqliteDataReader? reader = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM ShootingRange");

            if (reader == null)
            {
                throw new NullReferenceException("Reader is null");
            }

            ObservableCollection<ShootingRange> collection = new ObservableCollection<ShootingRange>();
            while (await reader.ReadAsync())
            {
                collection.Add(LoadNextFromReader(reader));
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
            SqliteDataReader? reader = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM ShootingRange");

            if (reader == null)
            {
                throw new NullReferenceException("Reader is null");
            }

            while (await reader.ReadAsync())
            {
                collection.Add(LoadNextFromReader(reader));
            }

            return collection;
        }


        public static ShootingRange LoadNextFromReader(SqliteDataReader reader)
        {
            return new ShootingRange()
            {
                ID = Convert.ToInt32(reader["ID"]),
                Name = Convert.ToString(reader["Name"]),
                Address = Convert.ToString(reader["Address"]),
                Latitude = Convert.ToDouble(reader["Latitude"]),
                Longitude = Convert.ToDouble(reader["Longitude"])
            };
        }
    }
}