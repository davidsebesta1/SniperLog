using Microsoft.Data.Sqlite;
using SniperLogNetworkLibrary;
using SniperLogNetworkLibrary.CommonLib;
using System.Data;

namespace SniperLog.Models
{
    public partial class Weather : ObservableObject, IDataAccessObject
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        private string? _clouds;

        [ObservableProperty]
        private double? _temperature;

        [ObservableProperty]
        private ushort? _pressure;

        [ObservableProperty]
        private byte? _humidity;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindInfo))]
        private byte? _windSpeed;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindInfo))]
        private ushort? _directionDegrees;

        public string? WindInfo => (DirectionDegrees == null || WindSpeed == null) ? null : $"{WindSpeed}m/s, {CardinalDirectionConverter.GetNameByDegree((int)DirectionDegrees)}";

        #endregion

        #region Ctors

        public Weather(int iD, string? clouds, double? temperature, ushort? pressure, byte? humidity, byte? windSpeed, ushort? directionDegrees)
        {
            ID = iD;
            Clouds = clouds;
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            WindSpeed = windSpeed;
            DirectionDegrees = directionDegrees;
        }

        public Weather(string? clouds, double? temperature, ushort? pressure, byte? humidity, byte? windSpeed, ushort? directionDegrees) : this(-1, clouds, temperature, pressure, humidity, windSpeed, directionDegrees) { }

        /// <summary>
        /// Constructor for Weather Response Message struct
        /// </summary>
        /// <param name="msg"></param>
        public Weather(WeatherResponseMessage msg) : this(-1, msg.Clouds, msg.Temperature, msg.Pressure, msg.Humidity, msg.WindSpeed, msg.DirectionDegrees) { }

        #endregion

        #region DAO

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new Weather(row);
        }

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
                ServicesHelper.GetService<DataCacherService<Weather>>().AddOrUpdate(this);
            }
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<Weather>>().Remove(this);
            }
        }

        #endregion
    }
}
