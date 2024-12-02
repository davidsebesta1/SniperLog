using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using SniperLogNetworkLibrary;

namespace SniperLog.ViewModels.Records
{
    public partial class WeatherEditPopupPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRecord _record;

        #region Editable properties

        [ObservableProperty]
        private string? _clouds;

        [ObservableProperty]
        private double? _temperature;

        [ObservableProperty]
        private ushort? _pressure;

        [ObservableProperty]
        private byte? _humidity;

        [ObservableProperty]
        private byte? _windSpeed;

        [ObservableProperty]
        private ushort? _directionDegrees;

        #endregion

        #region Ctor

        public WeatherEditPopupPageViewModel()
        {
            PageTitle = "Weather Settings";
        }

        #endregion

        #region Methods

        partial void OnRecordChanged(ShootingRecord? oldValue, ShootingRecord newValue)
        {
            if (newValue == null || newValue.ReferencedWeather == null)
            {
                Clouds = "";
                Temperature = 0.0;
                Pressure = 0;
                Humidity = 0;
                WindSpeed = 0;
                DirectionDegrees = 0;
                return;
            }

            Clouds = newValue.ReferencedWeather.Clouds;
            Temperature = newValue.ReferencedWeather.Temperature;
            Pressure = newValue.ReferencedWeather.Pressure;
            Humidity = newValue.ReferencedWeather.Humidity;
            WindSpeed = newValue.ReferencedWeather.WindSpeed;
            DirectionDegrees = newValue.ReferencedWeather.DirectionDegrees;
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task RefreshWeather()
        {
            try
            {
                WeatherResponseMessage msg = await Record.ReferencedShootingRange.GetCurrentWeather();

                if (msg.Equals(default(WeatherResponseMessage)))
                {
                    throw new Exception("No reponse from the server");
                }

                Clouds = msg.Clouds;
                Temperature = msg.Temperature;
                Pressure = msg.Pressure;
                Humidity = msg.Humidity;
                WindSpeed = msg.WindSpeed;
                DirectionDegrees = msg.DirectionDegrees;
            }
            catch (TimeoutException e)
            {
                //TODO: Handle?
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Okay");
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await MopupService.Instance.PopAsync();
        }

        [RelayCommand]
        private async Task Edit()
        {
            if (Record.ReferencedWeather == null)
            {
                Weather weather = new Weather(Clouds, Temperature, Pressure, Humidity, WindSpeed, DirectionDegrees);
                int id = await weather.SaveAsync();
                Record.Weather_ID = id;
                await Record.SaveAsync();
            }
            else
            {
                Record.ReferencedWeather.Clouds = Clouds;
                Record.ReferencedWeather.Temperature = Temperature;
                Record.ReferencedWeather.Pressure = Pressure;
                Record.ReferencedWeather.Humidity = Humidity;
                Record.ReferencedWeather.WindSpeed = WindSpeed;
                Record.ReferencedWeather.DirectionDegrees = DirectionDegrees;

                await Record.ReferencedWeather.SaveAsync();
            }


            await MopupService.Instance.PopAsync();
        }

        #endregion
    }
}
