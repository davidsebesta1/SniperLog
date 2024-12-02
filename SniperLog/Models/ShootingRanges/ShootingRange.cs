using Microsoft.Data.Sqlite;
using System.Data;
using SniperLogNetworkLibrary;
using SniperLog.Services.ConnectionToServer;
using SniperLogNetworkLibrary.Networking.Messages;
using SniperLog.Extensions;
using SniperLogNetworkLibrary.CommonLib;

namespace SniperLog.Models
{

    /// <summary>
    /// A class representing shooting range and its properties.<br></br>
    /// Background image is not automatically saved with the object itself and the method must be called seperately.
    /// All saved data are in Data/ShootingRanges/RangeName/
    /// </summary>
    public partial class ShootingRange : ObservableObject, IDataAccessObject, IImageSaveable, IEquatable<ShootingRange?>
    {
        public string ImageSavePath
        {
            get
            {
                return Path.Combine("Data", "ShootingRanges", Name, "backgroundimage.png");
            }
        }

        #region Constants

        #endregion

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string? _address;

        private double? _latitude;
        public double? Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;

                if (_longitude != null && _latitude != null)
                {
                    Location = new Location((double)_latitude, (double)_longitude);
                    OnPropertyChanged(nameof(LatLongString));
                }
            }
        }

        private double? _longitude;
        public double? Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;

                if (_longitude != null && _latitude != null)
                {
                    Location = new Location((double)_latitude, (double)_longitude);
                    OnPropertyChanged(nameof(LatLongString));
                }
            }
        }

        [DatabaseIgnore]
        public Location? Location { get; set; }

        [DatabaseIgnore]
        public string LatLongString => $"{Latitude}, {Longitude}";

        [DatabaseIgnore]
        [ObservableProperty]
        private WeatherResponseMessage _currentWeather;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FavStarImageName))]
        private bool _isMarkedAsFavourite;

        [DatabaseIgnore]
        public string FavStarImageName => IsMarkedAsFavourite ? _favImage : _normalImage;

        [DatabaseIgnore]
        private readonly string _normalImage = "stariconnormal.png";

        [DatabaseIgnore]
        private readonly string _favImage = "stariconfav.png";

        [DatabaseIgnore]
        public int? MaxRange => ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().MaxBy(n => n.RangeInMeters).RangeInMeters;

        [DatabaseIgnore]
        public double? AverageAltitude => ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().Average(n => n.Altitude);

        [DatabaseIgnore]
        public string FiringDirectionsStrings => string.Join(',', ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().Select(n => CardinalDirectionConverter.GetNameByDegree((int)n.DirectionToNorthDegrees)));

        [DatabaseIgnore]
        public string FiringDirectionsDegrees => string.Join(',', ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().Select(n => n.DirectionToNorthDegrees + "°"));

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
        public ShootingRange(int iD, string name, string? address, double? latitude, double? longitude, bool isMarkedAsFavourite)
        {
            ID = iD;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            IsMarkedAsFavourite = isMarkedAsFavourite;
        }

        /// <summary>
        /// New object constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="relativeImagePathFromAppdata"></param>
        public ShootingRange(string name, string? address, double? latitude, double? longitude, bool isMarkedAsFavourite) : this(-1, name, address, latitude, longitude, isMarkedAsFavourite)
        {

        }

        #endregion

        #region DAO Methods
        public async Task<int> SaveAsync()
        {
            try
            {
                if (ID == -1)
                {
                    ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId, GetSqliteParams(false));

                    await InitDefaultSubRangeForInstance();
                    return ID;
                }

                return await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQuery, GetSqliteParams(true));
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<ShootingRange>>().AddOrUpdate(this);
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
                DeleteImage();
                ServicesHelper.GetService<DataCacherService<ShootingRange>>().Remove(this);
            }
        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new ShootingRange(row);
        }

        #endregion

        #region Model Specific Methods

        /// <summary>
        /// Initializes default subrange for this range on first save
        /// </summary>
        /// <returns></returns>
        public async Task InitDefaultSubRangeForInstance()
        {
            SubRange subRange = new SubRange(ID, 0, 0, 0, 0, 'A');
            await subRange.SaveAsync();
        }

        /// <summary>
        /// Attempts to send a request while checking if the current weather is either null or the 5 minute time has elapsed locally
        /// </summary>
        /// <returns></returns>
        public async Task TrySendWeatherRequestMessage()
        {
            if (CurrentWeather.Equals(default(WeatherResponseMessage)))
            {
                await SendWeatherRequestMessage();
                return;
            }

            if ((CurrentWeather.TimeTaken - DateTime.UtcNow).Value.TotalMinutes > 5d)
            {
                await SendWeatherRequestMessage();
                return;
            }
        }

        /// <summary>
        /// Sends request to the server and asynchronously sets the CurrentWeather property
        /// </summary>
        /// <returns></returns>
        private async Task SendWeatherRequestMessage()
        {
            CurrentWeather = await GetCurrentWeather();
        }

        /// <summary>
        /// Gets the current weather on this range.
        /// </summary>
        /// <returns>Weather message from server. Or default it unable to connect.</returns>
        public async Task<WeatherResponseMessage> GetCurrentWeather()
        {
            if (Location == null)
                return default;


            INetworkMessage message = await ServicesHelper.GetService<ConnectionToDataServer>().SendRequest(new WeatherRequestMessage((double)Latitude, (double)Longitude));

            if (message == default || message == null)
                return default;

            return (WeatherResponseMessage)message;
        }

        partial void OnCurrentWeatherChanged(WeatherResponseMessage value)
        {
            OnPropertyChanged(nameof(CurrentWeather));
        }

        /// <summary>
        /// Returns a next prefix for a new subrange. If no subrange for this range exists, letter 'A' is returned.
        /// </summary>
        /// <returns>Prefix for next possible subrange</returns>
        public async Task<char> GetNextPrefix()
        {
            var subranges = await ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID);
            if (subranges.Count == 0)
                return 'A';

            return (char)(subranges.MaxBy(n => (int)n.Prefix).Prefix + 1);
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ShootingRange);
        }

        public bool Equals(ShootingRange? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ShootingRange? left, ShootingRange? right)
        {
            return EqualityComparer<ShootingRange>.Default.Equals(left, right);
        }

        public static bool operator !=(ShootingRange? left, ShootingRange? right)
        {
            return !(left == right);
        }

        #endregion
    }
}