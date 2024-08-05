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
    public partial class ShootingRange : ObservableObject, IDataAccessObject, IEquatable<ShootingRange?>
    {
        #region Constants

        public const string BackgroundImageFileName = "backgroundimage.png";

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundImgFullPath))]
        private string _backgroundImgPath;

        [DatabaseIgnore]
        public string BackgroundImgFullPath
        {
            get
            {
                string path = AppDataFileHelper.GetPathFromAppData(BackgroundImgPath);
                if (!Path.Exists(path))
                {
                    return "defaultbackground.png";
                }

                return path;
            }
        }

        [DatabaseIgnore]
        public int? MaxRange => ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().MaxBy(n => n.RangeInMeters).RangeInMeters;

        [DatabaseIgnore]
        public double? AverageAltitude => ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().Average(n => n.Altitude);

        [DatabaseIgnore]
        public string FiringDirectionsStrings => string.Join(',', ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().Where(n => n.DirectionToNorthDegrees != null).Select(n => CardinalDirectionConverter.GetNameByDegree((int)n.DirectionToNorthDegrees)));

        [DatabaseIgnore]
        public string FiringDirectionsDegrees => string.Join(',', ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID).GetAwaiter().GetResult().Where(n => n.DirectionToNorthDegrees != null).Select(n => n.DirectionToNorthDegrees + '°'));

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
        public ShootingRange(int iD, string name, string? address, double? latitude, double? longitude, bool isMarkedAsFavourite, string imageRelPath)
        {
            ID = iD;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            IsMarkedAsFavourite = isMarkedAsFavourite;
            BackgroundImgPath = imageRelPath;
        }

        /// <summary>
        /// New object constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="relativeImagePathFromAppdata"></param>
        public ShootingRange(string name, string? address, double? latitude, double? longitude, bool isMarkedAsFavourite, string imageRelPath) : this(-1, name, address, latitude, longitude, isMarkedAsFavourite, imageRelPath)
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
                string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRange", Name));
                if (Directory.Exists(dataPath))
                {
                    Directory.Delete(dataPath, true);
                }

                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
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
            SubRange subRange = new SubRange(ID, true, 0, 0, 0, 0, 'A', string.Empty);
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
            if (Location == null)
            {
                return;
            }

            INetworkMessage message = await ServicesHelper.GetService<ConnectionToDataServer>().SendRequest(new WeatherRequestMessage((double)Latitude, (double)Longitude));

            if (message == default || message == null)
            {
                return;
            }

            if (message is ErrorMessage errorMessage)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage.Message, "Okay");
                return;
            }

            CurrentWeather = default(WeatherResponseMessage);
            CurrentWeather = (WeatherResponseMessage)message;
        }

        partial void OnCurrentWeatherChanged(WeatherResponseMessage value)
        {
            OnPropertyChanged(nameof(CurrentWeather));
        }

        /// <summary>
        /// Saves the image to the predefined path
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task SaveImageAsync(FileStream stream)
        {
            string localPath = Path.Combine("Data", "ShootingRange", Name);
            string localFilePath = Path.Combine(localPath, BackgroundImageFileName);

            string fullDirPath = AppDataFileHelper.GetPathFromAppData(localPath);
            Directory.CreateDirectory(fullDirPath);

            string fullFilepath = Path.Combine(fullDirPath, BackgroundImageFileName);

            using (FileStream localFileStream = File.OpenWrite(fullFilepath))
            {
                await stream.CopyToAsync(localFileStream);
            }

            BackgroundImgPath = localFilePath;
        }

        /// <summary>
        /// Returns a next prefix for a new subrange. If no subrange for this range exists, letter 'A' is returned.
        /// </summary>
        /// <returns>Prefix for next possible subrange</returns>
        public async Task<char> GetNextPrefix()
        {
            var subranges = await ServicesHelper.GetService<DataCacherService<SubRange>>().GetAllBy(n => n.ShootingRange_ID == ID);
            if (subranges.Count == 0)
            {
                return 'A';
            }

            return (char)(subranges.MaxBy(n => (int)n.Prefix).Prefix + 1);
        }

        #endregion

        #region Equals

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