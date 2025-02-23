using BallisticCalculator;
using Gehtsoft.Measurements;
using SniperLogNetworkLibrary;

namespace SniperLog.Services.Ballistics
{
    public class BallisticCalculatorService
    {

        /// <summary>
        /// Calculates the offset for given conditions.
        /// </summary>
        /// <param name="weather">Weather to take into account.</param>
        /// <param name="distance">Distance to the target.</param>
        /// <returns></returns>
        public async Task<List<ClickOffset>> CalculateOffset(Firearm firearm, Models.Ammunition ammo, WeatherResponseMessage weather, int minRange, int maxRange, int step, double oneClickValue)
        {
            var t = (await ServicesHelper.GetService<DataCacherService<MuzzleVelocity>>().GetAllBy(n => n.Ammo_ID == ammo.ID && n.Firearm_ID == firearm.ID));
            double vel = t.Average(n => n.VelocityMS);

            BallisticCalculator.Ammunition ballisticAmmo = new BallisticCalculator.Ammunition(
                weight: new Measurement<WeightUnit>(ammo.ReferencedBullet.WeightGrams, WeightUnit.Gram),

                muzzleVelocity: new Measurement<VelocityUnit>(vel, VelocityUnit.MetersPerSecond),
                ballisticCoefficient: new BallisticCoefficient((double)ammo.ReferencedBullet.BCG1, DragTableId.G1),
                bulletDiameter: new Measurement<DistanceUnit>(ammo.ReferencedBullet.BulletDiameter, DistanceUnit.Millimeter),
                bulletLength: new Measurement<DistanceUnit>(ammo.ReferencedBullet.BulletLength, DistanceUnit.Millimeter));

            Sight sight = new Sight(
                sightHeight: new Measurement<DistanceUnit>((double)firearm.SightHeightCm, DistanceUnit.Centimeter),
                verticalClick: new Measurement<AngularUnit>(firearm.ReferencedFirearmSight.OneClickValue, firearm.ReferencedFirearmSight.ReferencedSightClickType.ClickTypeName == "MRAD" ? AngularUnit.MRad : AngularUnit.MOA),
                horizontalClick: new Measurement<AngularUnit>(firearm.ReferencedFirearmSight.OneClickValue, firearm.ReferencedFirearmSight.ReferencedSightClickType.ClickTypeName == "MRAD" ? AngularUnit.MRad : AngularUnit.MOA)
                );

            Rifling rifling = new Rifling(
                riflingStep: new Measurement<DistanceUnit>(12, DistanceUnit.Inch),
                direction: TwistDirection.Right);

            double min = (await ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>().GetAllBy(n => n.FirearmSight_ID == firearm.FirearmSight_ID)).Min(n => n.Distance);
            ZeroingParameters zero = new ZeroingParameters(
                distance: new Measurement<DistanceUnit>(min, DistanceUnit.Meter),
                ammunition: null,
                atmosphere: null
                );

            Rifle rifle = new Rifle(sight: sight, zero: zero, rifling: rifling);

            Atmosphere atmosphere = new Atmosphere(
                pressure: new Measurement<PressureUnit>((double)weather.Pressure * 100, PressureUnit.Pascal),
                pressureAtSeaLevel: false,
                altitude: new Measurement<DistanceUnit>(163, DistanceUnit.Meter),
                temperature: new Measurement<TemperatureUnit>((double)weather.Temperature, TemperatureUnit.Celsius),
                humidity: (double)(weather.Humidity / (double)100));

            TrajectoryCalculator calc = new TrajectoryCalculator();

            //shot parameters
            var shot = new ShotParameters()
            {
                MaximumDistance = new Measurement<DistanceUnit>(1000, DistanceUnit.Meter),
                Step = new Measurement<DistanceUnit>(50, DistanceUnit.Meter),
                //calculate sight angle for the specified zero distance
                SightAngle = calc.SightAngle(ballisticAmmo, rifle, atmosphere)
            };

            //define winds

            Wind[] wind =
            [
                new Wind()
                {
                    Direction = new Measurement<AngularUnit>((double)weather.DirectionDegrees, AngularUnit.Degree),
                    Velocity = new Measurement<VelocityUnit>((double)weather.WindSpeed, VelocityUnit.MetersPerSecond),
                    MaximumRange = new Measurement<DistanceUnit>(500, DistanceUnit.Meter),
                }
            ];


            //calculate trajectory
            TrajectoryPoint[] trajectory = calc.Calculate(ballisticAmmo, rifle, atmosphere, shot, wind);

            List<ClickOffset> offsets = new List<ClickOffset>();
            for (int i = minRange; i <= maxRange; i += step)
            {
                TrajectoryPoint point = null;
                int closestRange = int.MaxValue;
                foreach (TrajectoryPoint p in trajectory)
                {
                    double diff = Math.Abs(p.Distance.To(DistanceUnit.Meter).Value - i);
                    if (point == null || diff < closestRange)
                    {
                        point = p;
                        closestRange = (int)diff;
                    }
                }

                double d = point.DropAdjustment.To(AngularUnit.MRad).Value;
                double w = point.WindageAdjustment.To(AngularUnit.MRad).Value;

                if (point != null)
                    offsets.Add(new ClickOffset(Math.Abs((int)Math.Round(d / oneClickValue)), Math.Abs((int)Math.Round(w / oneClickValue)), (int)Math.Round(point.Distance.To(DistanceUnit.Meter).Value)));
            }

            return offsets;
        }

    }

    public enum ClickType
    {
        MOA,
        MRADs
    }
    /*
    var ammo = new Ammunition(
        weight: new Measurement<WeightUnit>(185, WeightUnit.Grain),
        muzzleVelocity: new Measurement<VelocityUnit>(822, VelocityUnit.MetersPerSecond),
        ballisticCoefficient: new BallisticCoefficient(0.319, DragTableId.G1),
        bulletDiameter: new Measurement<DistanceUnit>(0.308, DistanceUnit.Inch),
        bulletLength: new Measurement<DistanceUnit>(120, DistanceUnit.Millimeter));

    var sight = new Sight(
        sightHeight: new Measurement<DistanceUnit>(3.5, DistanceUnit.Inch),
        verticalClick: new Measurement<AngularUnit>(0.1, AngularUnit.MRad),
        horizontalClick: new Measurement<AngularUnit>(0.1, AngularUnit.MRad)
        );

    var rifling = new Rifling(
        riflingStep: new Measurement<DistanceUnit>(12, DistanceUnit.Inch),
        direction: TwistDirection.Right);

    var zero = new ZeroingParameters(
        distance: new Measurement<DistanceUnit>(100, DistanceUnit.Yard),
        ammunition: null,
        atmosphere: null
        );

    var rifle = new Rifle(sight: sight, zero: zero, rifling: rifling);

    var atmosphere = new Atmosphere(
        pressure: new Measurement<PressureUnit>(101200, PressureUnit.Pascal),
        pressureAtSeaLevel: false,
        altitude: new Measurement<DistanceUnit>(163, DistanceUnit.Foot),
        temperature: new Measurement<TemperatureUnit>(-2, TemperatureUnit.Celsius),
        humidity: 0.79);

    var calc = new TrajectoryCalculator();

    //shot parameters
    var shot = new ShotParameters()
    {
        MaximumDistance = new Measurement<DistanceUnit>(1000, DistanceUnit.Meter),
        Step = new Measurement<DistanceUnit>(50, DistanceUnit.Meter),
        //calculate sight angle for the specified zero distance
        SightAngle = calc.SightAngle(ammo, rifle, atmosphere)
    };

    //define winds

    Wind[] wind =
    [
new Wind()
{
    Direction = new Measurement<AngularUnit>(202, AngularUnit.Degree),
    Velocity = new Measurement<VelocityUnit>(2, VelocityUnit.MetersPerSecond),
    MaximumRange = new Measurement<DistanceUnit>(500, DistanceUnit.Meter),
}
    ];


    //calculate trajectory
    var trajectory = calc.Calculate(ammo, rifle, atmosphere, shot, wind);

    //print trajectory
    Console.WriteLine($"Distance;Drop;Wind");
    foreach (var point in trajectory)
    {
        Console.WriteLine($"{point.Distance:N0};{point.DropAdjustment.In(AngularUnit.MRad):N2};{point.WindageAdjustment.In(AngularUnit.MRad):N2}");
    }
}
    */
}
