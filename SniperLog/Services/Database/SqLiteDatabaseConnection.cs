using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using System.Data;

namespace SniperLog.Services.Database
{
    public class SqLiteDatabaseConnection
    {
        private readonly string _connectionString;

        private static SqLiteDatabaseConnection _instance;

        public static SqLiteDatabaseConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SqLiteDatabaseConnection();
                    InitializeSetup();
                }

                return _instance;
            }
        }

        private SqLiteDatabaseConnection()
        {
            _connectionString = $@"Data Source={AppDataFileHelper.GetPathFromAppData("SniperLogDatabase.db")};Pooling=True";
        }

        private static void InitializeSetup()
        {
            if (_instance == null)
            {
                throw new Exception("Instance for database connection is null");
            }

            _instance.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ShootingRange (ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,Name VARCHAR(50) UNIQUE NOT NULL,Address VARCHAR(100),Latitude DECIMAL(2,6) NOT NULL,Longitude DECIMAL(3,6) NOT NULL);");
            _instance.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SubRange(ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,ShootingRange_ID INTEGER NOT NULL,RangeInMeters INT NOT NULL,Altitude DOUBLE,DirectionToNorth DOUBLE,VerticalFiringOffsetDegrees DOUBLE,NotesRelativePathFromAppData VARCHAR(100),FOREIGN KEY (ShootingRange_ID) REFERENCES ShootingRange(ID));");
        }

        #region Queries Methods
        public async Task<int> ExecuteNonQueryAsync(string query, params SqliteParameter[] parameters)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return await command.ExecuteNonQueryAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return 0;
        }

        public int ExecuteNonQuery(string query, params SqliteParameter[] parameters)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                //Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return 0;
        }

        public async Task<DataTable?> ExecuteQueryAsync(string query, params SqliteParameter[] parameters)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        SqliteDataReader reader = await command.ExecuteReaderAsync();
                        DataTable table = new DataTable();
                        table.Load(reader);

                        return table;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return null;
        }

        public DataTable? ExecuteQuery(string query, params SqliteParameter[] parameters)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        SqliteDataReader reader = command.ExecuteReader();
                        DataTable table = new DataTable();
                        table.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return null;
        }

        public async Task<int> ExecuteScalarIntAsync(string query, params SqliteParameter[] parameters)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        object? result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int intValue))
                        {
                            return intValue;
                        }

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return 0;
        }

        public int ExecuteScalarInt(string query, params SqliteParameter[] parameters)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        object? result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int intValue))
                        {
                            return intValue;
                        }

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return 0;
        }

        #endregion
    }
}