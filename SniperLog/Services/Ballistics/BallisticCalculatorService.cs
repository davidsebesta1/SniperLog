using BallisticCalculator;
using Gehtsoft.Measurements;
using SniperLogNetworkLibrary;
using System.Collections.ObjectModel;

namespace SniperLog.Services.Ballistics;

/// <summary>
/// Service used to calculate ballistic offsets for firearms.s
/// </summary>
public class BallisticCalculatorService
{
    /// <summary>
    /// Calculates the offset for given conditions.
    /// </summary>
    /// <param name="firearm">The firearm.</param>
    /// <param name="ammo">The ammo the firearm shot.</param>
    /// <param name="weather">The weather to take into account.</param>
    /// <param name="minRange">Minimum range to return.</param>
    /// <param name="maxRange">Maximum range to return results.</param>
    /// <param name="step">Step to return results.</param>
    /// <param name="oneClickValue">One click value of the firearm's optic.</param>
    /// <returns>A new list of click offsets.</returns>
    public async Task<List<ClickOffset>> CalculateOffset(Firearm firearm, Models.Ammunition ammo, WeatherResponseMessage weather, int minRange, int maxRange, int step, double oneClickValue)
    {
        ObservableCollection<MuzzleVelocity> velocities = (await ServicesHelper.GetService<DataCacherService<MuzzleVelocity>>().GetAllBy(n => n.Ammo_ID == ammo.ID && n.Firearm_ID == firearm.ID));
        double vel = velocities.Average(n => n.VelocityMS);

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
            riflingStep: new Measurement<DistanceUnit>(int.Parse(firearm.RateOfTwist.Split(':')[1]), DistanceUnit.Inch),
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

        // Shot params
        ShotParameters shot = new ShotParameters()
        {
            MaximumDistance = new Measurement<DistanceUnit>(maxRange, DistanceUnit.Meter),
            Step = new Measurement<DistanceUnit>(step, DistanceUnit.Meter),

            //calculate sight angle for the specified zero distance
            SightAngle = calc.SightAngle(ballisticAmmo, rifle, atmosphere)
        };

        // Winds
        Wind[] wind =
        [
            new Wind()
                {
                    Direction = new Measurement<AngularUnit>((double)weather.DirectionDegrees, AngularUnit.Degree),
                    Velocity = new Measurement<VelocityUnit>(0, VelocityUnit.MetersPerSecond),
                    MaximumRange = new Measurement<DistanceUnit>(100, DistanceUnit.Meter),
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

/// <summary>
/// Click type of the optic.
/// </summary>
public enum ClickType
{
    /// <summary>
    /// Minute of angle.
    /// </summary>
    MOA,

    /// <summary>
    /// Miliradians.
    /// </summary>
    MRADs
}

