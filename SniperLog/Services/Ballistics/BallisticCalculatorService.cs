using SniperLogNetworkLibrary;

namespace SniperLog.Services.Ballistics
{
    public class BallisticCalculatorService
    {
        /// <summary>
        /// Earths gravity.
        /// </summary>
        public const double Gravity = 9.81d;

        /// <summary>
        /// Feet per second to meters/second conversion constant.
        /// </summary>
        public const double FtToMeters = 0.3048d;

        /// <summary>
        /// Specific gas constant for dry air (J/(kg·K)).
        /// </summary>
        public const double RDry = 287.05d;

        /// <summary>
        /// Specific gas constant for water vapor air (J/(kg·K)).
        /// </summary>
        public const double RVapor = 461.5d;

        /// <summary>
        /// Kg/m³ at sea level (dry air)
        /// </summary>
        public const double SeaLevelDensity = 1.225d;

        /// <summary>
        /// Value to add to C° in order to convert them to Kelvin.
        /// </summary>
        public const double CelsiusToKelvin = 273.15d;

        /// <summary>
        /// Calculates the offset for given conditions.
        /// </summary>
        /// <param name="weather">Weather to take into account.</param>
        /// <param name="distance">Distance to the target.</param>
        /// <param name="muzzleVelocityFps">Muzzle velocity in feet per second.</param>
        /// <param name="bc"></param>
        /// <param name="clickType"></param>
        /// <param name="clickValue"></param>
        /// <returns></returns>
        public ClickOffset CalculateOffset(WeatherResponseMessage weather, int distance, double muzzleVelocityFps, double bc, ClickType clickType, double clickValue)
        {
            double temperatureK = weather.Temperature.Value + CelsiusToKelvin;
            double muzzleVelocityMS = muzzleVelocityFps * FtToMeters;
            double airDensity = CalculateAirDensity(weather.Pressure.Value, weather.Temperature.Value, weather.Humidity.Value * 0.01d);
            double dragConstant = AdjustDragForAirDensity(airDensity);

            double timeOfFlight = CalculateTOF(muzzleVelocityMS, distance, bc, dragConstant, 3);

            double bulletDrop = CalculateDrop(timeOfFlight);
            double windDrift = CalculateWindDrift(weather.WindSpeed.Value, (double)weather.DirectionDegrees.Value, timeOfFlight);

            int elevationClicks;
            int windageClicks;
            if (clickType == ClickType.MOA)
            {
                double dropMOA = ConvertToMOA(bulletDrop, distance);
                double driftMOA = ConvertToMOA(windDrift, distance);
                elevationClicks = ConvertToMOAClicks(dropMOA, clickValue);
                windageClicks = ConvertToMOAClicks(driftMOA, clickValue);
            }
            else
            {
                elevationClicks = ConvertToMRADClicks(bulletDrop, clickValue);
                windageClicks = ConvertToMRADClicks(windDrift, clickValue);
            }

            return new ClickOffset(elevationClicks, windageClicks);
        }

        /// <summary>
        /// Calculates the air density used for ballistic calculation.
        /// </summary>
        /// <param name="pressureHPA">Pressure in hPa.</param>
        /// <param name="temperatureC">Temperature in degrees Celsius.</param>
        /// <param name="normalizedHumidity">Humidity in value 0f-1f.</param>
        /// <returns>Pressure in kg/m³.</returns>
        public double CalculateAirDensity(double pressureHPA, double temperatureC, double normalizedHumidity)
        {
            // Convert inputs
            double temperatureK = temperatureC + CelsiusToKelvin; // Temperature in Kelvin
            double pressurePa = pressureHPA * 100;               // Pressure in Pascals

            // Calculate saturation vapor pressure (Magnus-Tetens approximation)
            double saturationVaporPressure = CalculatePSaturation(temperatureC); // Pa

            // Calculate actual vapor pressure
            double vaporPressure = normalizedHumidity * saturationVaporPressure; // Pa

            // Specific gas constant for moist air
            double f = vaporPressure / pressurePa; // Molar fraction of water vapor
            double specificGasConstant = RDry * (1 - f * (1 - RDry / RVapor));

            // Air density using the Wikipedia formula
            double airDensity = pressurePa / (specificGasConstant * temperatureK);

            return airDensity;
        }

        /// <summary>
        /// Calculates Saturation Vapor Pressure (Pa) using Magnus-Tetens approximation.
        /// </summary>
        /// <param name="temperatureC">Temperature in degrees Celsius.</param>
        /// <returns>Saturation Vapor Pressure in Pascals.</returns>
        public double CalculatePSaturation(double temperatureC)
        {
            return 6.1078 * Math.Pow(10, (7.5 * temperatureC) / (temperatureC + 237.3)) * 100;
        }

        /// <summary>
        /// Function to adjust drag constant for air density.
        /// </summary>
        /// <param name="airDensity">The air density in kg/m³.</param>
        /// <returns>Adjusted air density in kg/m³.</returns>
        public double AdjustDragForAirDensity(double airDensity)
        {
            return 0.5 * airDensity / SeaLevelDensity;
        }

        /// <summary>
        /// Calculates the Time of Flight (TOF).
        /// </summary>
        /// <param name="muzzleVelocity">Muzzle velocity of the bullet in m/s.</param>
        /// <param name="distance">Distance to the target in meters.</param>
        /// <param name="bc">Ballistic coeficient.</param>
        /// <param name="dragConstant">Drag constant. Usually either G1 or G7.</param>
        /// <returns></returns>
        public double CalculateTOF(double muzzleVelocity, double distance, double bc, double dragConstant, double launchAngle = 0, double airDensity = 1.225)
        {
            double time = 0.0;        // Time elapsed in seconds
            double x = 0.0;           // Horizontal position in meters
            double y = 0.0;           // Vertical position in meters
            double vx = muzzleVelocity * Math.Cos(DegreeToRadian(launchAngle)); // Horizontal velocity
            double vy = muzzleVelocity * Math.Sin(DegreeToRadian(launchAngle)); // Vertical velocity
            double deltaT = 0.001;    // Small time step for integration (in seconds)

            while (x < distance)
            {
                // Update velocities (gravity affects vertical velocity only)
                vy -= Gravity * deltaT;

                // Update positions
                x += vx * deltaT;
                y += vy * deltaT;

                // Update time
                time += deltaT;
            }

            return time; // Total time of flight in seconds
        }

        /// <summary>
        /// Calculates the bullet drop in meters.
        /// </summary>
        /// <param name="timeOfFlight">Time of flight in seconds.</param>
        /// <returns>Bullet drop in meters.</returns>
        public double CalculateDrop(double timeOfFlight)
        {
            return 0.5 * Gravity * Math.Pow(timeOfFlight, 2);
        }

        /// <summary>
        /// Calculates Wind Drift in meters.
        /// </summary>
        /// <param name="windSpeed">Wind speed in m/s.</param>
        /// <param name="windAngle">Wind angle in degrees (direction)</param>
        /// <param name="timeOfFlight">Time of flight of the bullet in seconds.</param>
        /// <returns>Wind drift in meters.</returns>
        public double CalculateWindDrift(double windSpeed, double windAngle, double timeOfFlight)
        {
            double windComponent = windSpeed * Math.Sin(DegreeToRadian(windAngle));
            return windComponent * timeOfFlight;
        }

        /// <summary>
        /// Converts meters of drop/drift to MOA (Minute of angle).
        /// </summary>
        /// <param name="correction">Correction in meters.</param>
        /// <param name="distance">Distance to the target.</param>
        /// <returns>Correction to the target in MOA.</returns>
        public double ConvertToMOA(double correction, double distance)
        {
            return (correction / distance) * 100d * 60d / 2.54d; // 1 MOA = ~2.54 cm at 100 m
        }

        /// <summary>
        /// Converts meters of drop/drift to MRAD clicks.
        /// </summary>
        /// <param name="correction">Correction in meters.</param>
        /// <param name="distance">Distance in meters.</param>
        /// <returns>Clicks to adjust the optic. Rounded down to nearest whole number.</returns>
        public int ConvertToMRADClicks(double correction, double distance)
        {
            return (int)Math.Floor((correction * 0.01d) / (distance * 0.01d));
        }

        /// <summary>
        /// Converts MOA to Scope Clicks.
        /// </summary>
        /// <param name="adjustmentMOA">MOA value.</param>
        /// <param name="clickValue">One click value of the scape.</param>
        /// <returns>Clicks to adjust the optic. Rounded DOWN to nearest whole number.</returns>
        public int ConvertToMOAClicks(double adjustmentMOA, double clickValue)
        {
            return (int)Math.Floor(adjustmentMOA / clickValue);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180d;
        }
    }

    public enum ClickType
    {
        MOA,
        MRADs
    }
}
