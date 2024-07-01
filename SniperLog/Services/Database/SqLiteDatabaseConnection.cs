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

        public static void Init()
        {
            Instance.ExecuteNonQuery("");
        }

        private SqLiteDatabaseConnection()
        {
            _connectionString = $@"Data Source={AppDataFileHelper.GetPathFromAppData("SniperLogDatabase.db")};Pooling=True";
        }

        private static async void InitializeSetup()
        {
            try
            {
                if (_instance == null)
                {
                    throw new Exception("Instance for database connection is null");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            }
        }

        #region Queries Methods
        public async Task<int> ExecuteNonQueryAsync(string query, params SqliteParameter[] parameters)
        {
            if (string.IsNullOrEmpty(query)) return 0;

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
                        };

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
            if (string.IsNullOrEmpty(query)) return 0;

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
            if (string.IsNullOrEmpty(query)) return null;

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
            if (string.IsNullOrEmpty(query)) return null;

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
                        return table;
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
            if (string.IsNullOrEmpty(query)) return 0;

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
            if (string.IsNullOrEmpty(query)) return 0;

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