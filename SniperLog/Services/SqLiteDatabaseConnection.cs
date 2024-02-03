using Microsoft.Data.Sqlite;

namespace SniperLog.Services
{
    public class SqLiteDatabaseConnection
    {
        private readonly string _connectionString = @"Data Source=c:\SniperLogDatabase.db;Version=3;Pooling=True;Max Pool Size=100;";

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

        private static void InitializeSetup()
        {
            if (_instance == null)
            {
                throw new Exception("Instance for database connection is null");
            }

            _instance.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ShootingRange (ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,Name VARCHAR(50) UNIQUE,Address VARCHAR(100),Latitude DOUBLE NOT NULL,Longitude DOUBLE NOT NULL);");
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
                Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return 0;
        }

        public async Task<SqliteDataReader?> ExecuteQueryAsync(string query, params SqliteParameter[] parameters)
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

                        return await command.ExecuteReaderAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }

            return null;
        }

        public SqliteDataReader? ExecuteQuery(string query, params SqliteParameter[] parameters)
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

                        return command.ExecuteReader();
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