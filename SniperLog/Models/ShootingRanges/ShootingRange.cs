using Microsoft.Data.Sqlite;
using SniperLog.Services.Database;
using System.Data;
using SniperLogNetworkLibrary;
using SniperLog.Services.ConnectionToServer;
using SniperLogNetworkLibrary.Networking.Messages;
using System.Resources;
using SniperLog.Extensions;

namespace SniperLog.Models
{

    /// <summary>
    /// A class representing shooting range and its properties.<br></br>
    /// Background image is not automatically saved with the object itself and the method must be called seperately.
    /// </summary>
    public partial class ShootingRange : ObservableObject, IDataAccessObject, IEquatable<ShootingRange?>
    {
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
                }
            }
        }

        [DatabaseIgnore]
        public Location? Location { get; set; }

        [DatabaseIgnore]
        [ObservableProperty]
        private WeatherResponseMessage? _currentWeather;

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
                string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", Name));
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

        public async Task InitDefaultSubRangeForInstance()
        {
            SubRange subRange = new SubRange(ID, true, 0, 0, 0, 0);
            await subRange.SaveAsync();
        }

        public async Task TrySendWeatherRequestMessage()
        {
            if (CurrentWeather == null)
            {
                await SendWeatherRequestMessage();
                return;
            }

            if ((CurrentWeather.Value.TimeTaken - DateTime.UtcNow).TotalMinutes > 5d)
            {
                await SendWeatherRequestMessage();
                return;
            }
        }

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

            CurrentWeather = (WeatherResponseMessage)message;
        }

        partial void OnCurrentWeatherChanged(WeatherResponseMessage? value)
        {
            OnPropertyChanged(nameof(CurrentWeather));
        }

        private async Task SaveImageAsync(byte[] data)
        {
            string fulPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", Name, "backgroundimage.png"));

            await File.WriteAllBytesAsync(fulPath, data);
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