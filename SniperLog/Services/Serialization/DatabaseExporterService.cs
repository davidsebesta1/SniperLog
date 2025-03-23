using CommunityToolkit.Maui.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SniperLog.Extensions.WrapperClasses;
using System.Collections.ObjectModel;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SniperLog.Services.Serialization;

/// <summary>
/// Exporter/importer service.
/// </summary>
public class DatabaseExporterService
{
    /// <summary>
    /// Exports firearm to json to a specified path.
    /// </summary>
    /// <param name="firearmId">Id of the firearm.</param>
    /// <param name="outputPath">Target output path.</param>
    public static async Task ExportFirearmToJson(int firearmId, string outputPath)
    {
        DataCacherService<Firearm> firearmService = ServicesHelper.GetService<DataCacherService<Firearm>>();
        Firearm firearm = await firearmService.GetFirstBy(f => f.ID == firearmId);

        if (firearm == null)
        {
            await Shell.Current.DisplayAlert("Err", "Firearm not found.", "Okay");
            return;
        }

        // Retrieve related data
        DataCacherService<Manufacturer> manufacturerService = ServicesHelper.GetService<DataCacherService<Manufacturer>>();
        DataCacherService<FirearmCaliber> caliberService = ServicesHelper.GetService<DataCacherService<FirearmCaliber>>();
        DataCacherService<FirearmSight> sightService = ServicesHelper.GetService<DataCacherService<FirearmSight>>();
        DataCacherService<FirearmSightSetting> sightSettingService = ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>();
        DataCacherService<Ammunition> ammoService = ServicesHelper.GetService<DataCacherService<Ammunition>>();
        DataCacherService<ShootingRecord> shootingRecordService = ServicesHelper.GetService<DataCacherService<ShootingRecord>>();
        DataCacherService<ShootingRecordImage> imageService = ServicesHelper.GetService<DataCacherService<ShootingRecordImage>>();

        Manufacturer manufacturer = firearm.ReferencedManufacturer;
        FirearmCaliber caliber = firearm.ReferencedFirearmCaliber;
        FirearmSight sight = firearm.ReferencedFirearmSight;
        ObservableCollection<ShootingRecord> shootingRecords = await shootingRecordService.GetAllBy(sr => sr.Firearm_ID == firearm.ID);
        ObservableCollection<Ammunition> ammunitionList = await ammoService.GetAllBy(a => shootingRecords.Any(n => n.Ammo_ID == a.ID));

        try
        {
            Manufacturer manu = firearm.ReferencedManufacturer;
            var firearmData = new
            {
                firearm.Name,
                firearm.Model,
                Type = firearm.ReferencedFirearmType.TypeName,
                firearm.SerialNumber,
                firearm.TotalLengthMm,
                firearm.BarrelLengthInch,
                firearm.RateOfTwist,
                firearm.Weight,
                firearm.HandednessForLeft,
                firearm.SightHeightCm,
                Manufacturer = manu != null ? new
                {
                    Type = manu.ReferencedManufacturerType.Name,
                    Name = manu.Name,
                    Country = manu.ReferencedCountry.Name
                } : null,
                caliber?.Caliber,
                Sight = sight != null ? new
                {
                    sight.Name,
                    sight.ReferencedSightClickType.ClickTypeName,
                    sight.OneClickValue,
                    Manufacturer = sight.ReferencedManufacturer != null ? new
                    {
                        Type = sight.ReferencedManufacturer.ReferencedManufacturerType.Name,
                        Name = sight.ReferencedManufacturer.Name,
                        Country = sight.ReferencedManufacturer.ReferencedCountry.Name
                    } : null,
                    SightReticleName = sight.ReferencedSightReticle.Name,
                    ZeroSettings = (await sightSettingService.GetAllBy(n => n.FirearmSight_ID == sight.ID)).Select(n => new
                    {
                        n.Distance,
                        n.ElevationValue,
                        n.WindageValue,
                    }),
                } : null,
                Ammunition = ammunitionList.Select(a =>
                {
                    Bullet refBullet = a.ReferencedBullet;
                    return new
                    {
                        a.TotalLengthMm,
                        a.GunpowderAmountGrams,
                        Bullet = refBullet != null ? new
                        {
                            Caliber = refBullet.ReferencedFirearmCaliber.Caliber,
                            WeightGrams = refBullet.WeightGrams,
                            Diameter = refBullet.BulletDiameter,
                            Length = refBullet.BulletLength,
                            BC1 = refBullet.BCG1,
                            BC7 = refBullet.BCG7,
                            Manufacturer = refBullet.ReferencedManufacturer != null ? new
                            {
                                Type = refBullet.ReferencedManufacturer.ReferencedManufacturerType.Name,
                                Name = refBullet.ReferencedManufacturer.Name,
                                Country = refBullet.ReferencedManufacturer.ReferencedCountry.Name
                            } : null
                        } : null,
                    };
                }).ToList(),
                ShootingRecords = shootingRecords.Select(async sr =>
                {
                    var images = await imageService.GetAllBy(img => img.ShootingRecord_ID == sr.ID);
                    Weather weather = sr.ReferencedWeather;
                    int ammoIdx = ammunitionList.IndexOf(sr.ReferencedAmmunition);
                    return new
                    {
                        sr.ReferencedShootingRange.Name,
                        sr.ReferencedSubRange.Prefix,
                        sr.Distance,
                        sr.TimeTaken,
                        sr.ElevationClicksOffset,
                        sr.WindageClicksOffset,
                        sr.NotesText,
                        AmmoIdx = ammoIdx,
                        Weather = weather != null ? new
                        {
                            weather.Temperature,
                            weather.Pressure,
                            weather.Humidity,
                            weather.WindSpeed,
                            weather.DirectionDegrees,
                            weather.Clouds
                        } : null,
                        Images = images.Select(img => new
                        {
                            Base64Image = ConvertImageToBase64(img)
                        }).ToList(),
                    };
                }).Select(t => t.Result).ToList()
            };

            string json = JsonSerializer.Serialize(firearmData, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(outputPath, json);
        }
        catch (Exception e)
        {

            await Shell.Current.DisplayAlert("Err", e.Message, "Okay");
        }

    }

    /// <summary>
    /// Imports firearm and any related objects with it from a specified path.
    /// </summary>
    /// <param name="inputPath">Target input path.</param>
    public static async Task ImportFirearmFromJson(string inputPath)
    {
        if (!File.Exists(inputPath))
        {
            await Shell.Current.DisplayAlert("Error", "File not found.", "Okay");
            return;
        }

        string json = await File.ReadAllTextAsync(inputPath);
        dynamic firearmData = JsonConvert.DeserializeObject(json);

        DataCacherService<Country> countryService = ServicesHelper.GetService<DataCacherService<Country>>();

        DataCacherService<Firearm> firearmService = ServicesHelper.GetService<DataCacherService<Firearm>>();
        DataCacherService<FirearmType> firearmTypeService = ServicesHelper.GetService<DataCacherService<FirearmType>>();

        DataCacherService<Manufacturer> manufacturerService = ServicesHelper.GetService<DataCacherService<Manufacturer>>();
        DataCacherService<ManufacturerType> manufacturerTypeService = ServicesHelper.GetService<DataCacherService<ManufacturerType>>();

        DataCacherService<FirearmSight> sightService = ServicesHelper.GetService<DataCacherService<FirearmSight>>();
        DataCacherService<SightClickType> sightClickTypeService = ServicesHelper.GetService<DataCacherService<SightClickType>>();
        DataCacherService<FirearmSightSetting> sightSettingService = ServicesHelper.GetService<DataCacherService<FirearmSightSetting>>();
        DataCacherService<SightReticle> sightReticleService = ServicesHelper.GetService<DataCacherService<SightReticle>>();

        DataCacherService<FirearmCaliber> caliberService = ServicesHelper.GetService<DataCacherService<FirearmCaliber>>();
        DataCacherService<Ammunition> ammoService = ServicesHelper.GetService<DataCacherService<Ammunition>>();
        DataCacherService<Bullet> bulletService = ServicesHelper.GetService<DataCacherService<Bullet>>();

        DataCacherService<ShootingRecord> shootingRecordService = ServicesHelper.GetService<DataCacherService<ShootingRecord>>();
        DataCacherService<ShootingRecordImage> imageService = ServicesHelper.GetService<DataCacherService<ShootingRecordImage>>();
        DataCacherService<Weather> weatherService = ServicesHelper.GetService<DataCacherService<Weather>>();

        DataCacherService<ShootingRange> rangeService = ServicesHelper.GetService<DataCacherService<ShootingRange>>();
        DataCacherService<SubRange> subRangeService = ServicesHelper.GetService<DataCacherService<SubRange>>();

        string firearmName = firearmData["Name"];
        if (string.IsNullOrEmpty(firearmName))
        {
            await Shell.Current.DisplayAlert("Error", "Invalid firearm data.", "Okay");
            return;
        }

        JArray ammunitionsArray = firearmData.Ammunition as JArray;
        List<Ammunition> ammo = new List<Ammunition>(ammunitionsArray.Count);

        foreach (JToken ammoToken in ammunitionsArray)
        {
            double totalLength = ammoToken["TotalLengthMm"].ToObject<double>();
            double gunpowderAmount = ammoToken["GunpowderAmountGrams"].ToObject<double>();

            JToken bulletToken = ammoToken["Bullet"];
            if (bulletToken != null)
            {
                string caliber = bulletToken["Caliber"].ToString();
                double weight = bulletToken["WeightGrams"].ToObject<double>();
                double diameter = bulletToken["Diameter"].ToObject<double>();
                double length = bulletToken["Length"].ToObject<double>();
                double bc1 = bulletToken["BC1"].ToObject<double>();
                double bc7 = bulletToken["BC7"].ToObject<double>();

                JToken manufacturerToken = bulletToken["Manufacturer"];
                Manufacturer bulletManufacturer = null;

                if (manufacturerToken != null)
                {
                    string manufacturerName = manufacturerToken["Name"].ToString();
                    string manufacturerType = manufacturerToken["Type"].ToString();
                    string manufacturerCountryName = manufacturerToken["Country"].ToString();

                    ManufacturerType existingType = await manufacturerTypeService.GetFirstBy(t => t.Name == manufacturerType);
                    Country country = await countryService.GetFirstBy(n => n.Name == manufacturerCountryName);

                    bulletManufacturer = await manufacturerService.GetFirstBy(m => m.Name == manufacturerName);
                    if (bulletManufacturer == null)
                    {
                        bulletManufacturer = new Manufacturer(country.ID, existingType.ID, manufacturerName);
                        await bulletManufacturer.SaveAsync();
                    }
                }

                FirearmCaliber existingCaliber = await caliberService.GetFirstBy(c => c.Caliber == caliber);
                if (existingCaliber == null)
                {
                    existingCaliber = new FirearmCaliber(caliber);
                    await existingCaliber.SaveAsync();
                }

                Bullet? existingBullet = await bulletService.GetFirstBy(b =>
                    b.WeightGrams == weight &&
                    b.BulletDiameter == diameter &&
                    b.BulletLength == length &&
                    b.BCG1 == bc1 &&
                    b.BCG7 == bc7 &&
                    b.Caliber_ID == existingCaliber.ID &&
                    (b.Manufacturer_ID == bulletManufacturer?.ID || bulletManufacturer == null)
                );

                if (existingBullet == null)
                {
                    existingBullet = new Bullet(existingCaliber.ID, bulletManufacturer.ID, weight, diameter, length, bc1, bc7);
                    await existingBullet.SaveAsync();
                }

                Ammunition? existingAmmo = await ammoService.GetFirstBy(a =>
                    a.TotalLengthMm == totalLength &&
                    a.GunpowderAmountGrams == gunpowderAmount &&
                    a.Bullet_ID == existingBullet.ID
                );

                if (existingAmmo == null)
                {
                    Ammunition newAmmo = new Ammunition(existingBullet.ID, totalLength, gunpowderAmount);
                    await newAmmo.SaveAsync();
                }
            }
        }

        Firearm existingFirearm = await firearmService.GetFirstBy(f => f.Name == firearmName);
        if (existingFirearm != null)
        {
            bool result = await Shell.Current.DisplayAlert("Warning", $"Firearm with the same name already exists, would you like the import {firearmData.ShootingRecords.Count} shooting records?", "Yes", "No");
            if (!result)
                return;

            ImportLogs(firearmData, existingFirearm, ammo);
            return;
        }
        else
        {
            string name = firearmData["Name"]?.ToString();
            string typeName = firearmData["Type"]?.ToString();
            string model = firearmData["Model"]?.ToString();
            string serialNumber = firearmData["SerialNumber"]?.ToString();
            double totalLengthMm = firearmData["TotalLengthMm"]?.ToObject<double>() ?? 0;
            double barrelLengthInch = firearmData["BarrelLengthInch"]?.ToObject<double>() ?? 0;
            string rateOfTwist = firearmData["RateOfTwist"]?.ToString();
            double weight = firearmData["Weight"]?.ToObject<double>() ?? 0;
            bool handednessForLeft = firearmData["HandednessForLeft"]?.ToObject<bool>() ?? false;
            double sightHeightCm = firearmData["SightHeightCm"]?.ToObject<double>() ?? 0;

            string caliberName = firearmData["Caliber"]?.ToString();

            JToken manufacturerToken = firearmData["Manufacturer"];
            Manufacturer firearmManufacturer = null;

            FirearmType firearmType = await firearmTypeService.GetFirstBy(n => n.TypeName == typeName);

            if (firearmType == null)
            {
                firearmType = new FirearmType(typeName);
                await firearmType.SaveAsync();
            }

            if (manufacturerToken != null)
            {
                string manufacturerName = manufacturerToken["Name"].ToString();
                string manufacturerType = manufacturerToken["Type"].ToString();
                string manufacturerCountryName = manufacturerToken["Country"].ToString();

                ManufacturerType existingType = await manufacturerTypeService.GetFirstBy(t => t.Name == manufacturerType);
                Country country = await countryService.GetFirstBy(n => n.Name == manufacturerCountryName);

                firearmManufacturer = await manufacturerService.GetFirstBy(m => m.Name == manufacturerName);
                if (firearmManufacturer == null)
                {
                    firearmManufacturer = new Manufacturer(country.ID, existingType.ID, manufacturerName);
                    await firearmManufacturer.SaveAsync();
                }
            }


            FirearmCaliber caliber = await caliberService.GetFirstBy(n => n.Caliber == caliberName);
            if (caliber == null)
            {
                caliber = new FirearmCaliber(caliberName);
                await caliber.SaveAsync();
            }


            JToken sightToken = firearmData["Sight"];
            FirearmSight sight = null;

            if (sightToken != null)
            {
                string sightName = sightToken["Name"]?.ToString();
                string clickTypeName = sightToken["ClickTypeName"]?.ToString();
                double oneClickValue = sightToken["OneClickValue"]?.ToObject<double>() ?? 0;
                string sightReticleName = sightToken["SightReticleName"]?.ToString();

                JToken sightManufacturerToken = sightToken["Manufacturer"];
                Manufacturer sightManufacturer = null;

                if (manufacturerToken != null)
                {
                    string manufacturerTypeName = sightManufacturerToken["Type"]?.ToString();
                    string manufacturerName = sightManufacturerToken["Name"]?.ToString();
                    string countryName = sightManufacturerToken["Country"]?.ToString();

                    ManufacturerType manufacturerType = await manufacturerTypeService.GetFirstBy(t => t.Name == manufacturerTypeName);
                    if (manufacturerType == null)
                    {
                        manufacturerType = new ManufacturerType(manufacturerTypeName);
                        await manufacturerType.SaveAsync();
                    }

                    Country country = await countryService.GetFirstBy(c => c.Name == countryName);

                    sightManufacturer = await manufacturerService.GetFirstBy(m => m.Name == manufacturerName);
                    if (sightManufacturer == null)
                    {
                        sightManufacturer = new Manufacturer(country.ID, manufacturerType.ID, manufacturerName);
                        await sightManufacturer.SaveAsync();
                    }
                }

                SightClickType sightClickType = await sightClickTypeService.GetFirstBy(s => s.ClickTypeName == clickTypeName);
                if (sightClickType == null)
                {
                    sightClickType = new SightClickType(clickTypeName);
                    await sightClickType.SaveAsync();
                }

                SightReticle sightReticle = await sightReticleService.GetFirstBy(r => r.Name == sightReticleName);
                if (sightReticle == null)
                {
                    sightReticle = new SightReticle(sightReticleName);
                    await sightReticle.SaveAsync();
                }

                sight = await sightService.GetFirstBy(s => s.Name == sightName);
                if (sight == null)
                {
                    sight = new FirearmSight(sightClickType.ID, sightManufacturer.ID, sightReticle.ID, sightName, oneClickValue);
                    await sight.SaveAsync();
                }

                JArray zeroSettingsArray = sightToken["ZeroSettings"] as JArray;
                if (zeroSettingsArray != null)
                {
                    foreach (JToken zeroSetting in zeroSettingsArray)
                    {
                        int distance = zeroSetting["Distance"]?.ToObject<int>() ?? 0;
                        int elevationValue = zeroSetting["ElevationValue"]?.ToObject<int>() ?? 0;
                        int windageValue = zeroSetting["WindageValue"]?.ToObject<int>() ?? 0;

                        FirearmSightSetting existingSetting = await sightSettingService.GetFirstBy(z => z.FirearmSight_ID == sight.ID && z.Distance == distance);

                        if (existingSetting == null)
                        {
                            FirearmSightSetting newSetting = new FirearmSightSetting(sight.ID, distance, elevationValue, windageValue);
                            await newSetting.SaveAsync();
                        }
                    }
                }
            }

            Firearm firearm = new Firearm(firearmType.ID, firearmManufacturer.ID, caliber.ID, sight.ID, name, model, serialNumber, totalLengthMm, barrelLengthInch, rateOfTwist, weight, handednessForLeft, sightHeightCm);
            await firearm.SaveAsync();

            ImportLogs(firearmData, firearm, ammo);
        }

    }

    private static async Task ImportLogs(dynamic obj, Firearm firearm, List<Ammunition> ammo)
    {
        DataCacherService<Firearm> firearmService = ServicesHelper.GetService<DataCacherService<Firearm>>();
        DataCacherService<Manufacturer> manufacturerService = ServicesHelper.GetService<DataCacherService<Manufacturer>>();
        DataCacherService<FirearmCaliber> caliberService = ServicesHelper.GetService<DataCacherService<FirearmCaliber>>();
        DataCacherService<FirearmSight> sightService = ServicesHelper.GetService<DataCacherService<FirearmSight>>();
        DataCacherService<Ammunition> ammoService = ServicesHelper.GetService<DataCacherService<Ammunition>>();
        DataCacherService<Bullet> bulletService = ServicesHelper.GetService<DataCacherService<Bullet>>();
        DataCacherService<ShootingRecord> shootingRecordService = ServicesHelper.GetService<DataCacherService<ShootingRecord>>();
        DataCacherService<ShootingRecordImage> imageService = ServicesHelper.GetService<DataCacherService<ShootingRecordImage>>();
        DataCacherService<Weather> weatherService = ServicesHelper.GetService<DataCacherService<Weather>>();
        DataCacherService<ShootingRange> rangeService = ServicesHelper.GetService<DataCacherService<ShootingRange>>();
        DataCacherService<SubRange> subRangeService = ServicesHelper.GetService<DataCacherService<SubRange>>();

        JArray shootingRecordsArray = obj.ShootingRecords as JArray;

        foreach (JToken recordToken in shootingRecordsArray)
        {
            string name = recordToken["Name"].ToString();
            string prefix = recordToken["Prefix"].ToString();
            long timeTaken = recordToken["TimeTaken"].ToObject<long>();
            int distance = recordToken["Distance"].ToObject<int>();
            int elevOffset = recordToken["ElevationClicksOffset"].ToObject<int>();
            int windOffset = recordToken["WindageClicksOffset"].ToObject<int>();
            string notes = recordToken["NotesText"].ToString();
            int ammoIdx = recordToken["AmmoIdx"].ToObject<int>();

            ShootingRange range = await rangeService.GetFirstBy(n => n.Name == name);
            if (range == null)
            {
                range = new ShootingRange(name, string.Empty, 0d, 0d, false);
                await range.SaveAsync();
            }

            SubRange subRange = await subRangeService.GetFirstBy(n => n.ShootingRange_ID == range.ID && n.Prefix == prefix[0]);

            if (subRange == null)
            {
                subRange = new SubRange(range.ID, 0, 0, 0, 0, prefix[0]);
                await subRange.SaveAsync();
            }

            JToken weatherToken = recordToken["Weather"];

            Weather weather = null;
            if (weatherToken != null)
            {
                string clouds = weatherToken["Clouds"]?.ToString();
                double? temp = weatherToken["Temperature"]?.ToObject<double>();
                ushort? pressure = weatherToken["Pressure"]?.ToObject<ushort>();
                byte? humidity = weatherToken["Humidity"]?.ToObject<byte>();
                byte? windSpeed = weatherToken["WindSpeed"]?.ToObject<byte>();
                ushort? directionDegrees = weatherToken["DirectionDegrees"]?.ToObject<ushort>();

                weather = new Weather(clouds, temp, pressure, humidity, windSpeed, directionDegrees);
            }

            ShootingRecord record = new ShootingRecord(range.ID, subRange.ID, firearm.ID, ammo[ammoIdx].ID, weather.ID, elevOffset, windOffset, distance, timeTaken);
            await record.SaveAsync();
            if (!string.IsNullOrEmpty(notes))
                await record.SaveNotesAsync(notes);

            JArray imagesArray = recordToken["Images"] as JArray;

            foreach (JToken imgToken in imagesArray)
            {
                byte[] imgBytes = ConvertBase64ToImage(imgToken.ToString());

                string tmpSavePath = Path.Combine(FileSystem.Current.CacheDirectory, Guid.NewGuid().ToString() + ".png");
                await File.WriteAllBytesAsync(tmpSavePath, imgBytes);

                DrawableImagePaths paths = new DrawableImagePaths(tmpSavePath);
                await record.SaveImageAsync(paths);
            }

        }

    }

    private static string ConvertImageToBase64(ShootingRecordImage image)
    {
        DrawableImagePaths paths = new DrawableImagePaths(image);

        string str = paths.CombinedImageSrc.ToString().Split(' ')[1];
        byte[] imageBytes = File.ReadAllBytes(str);
        return Convert.ToBase64String(imageBytes);
    }

    private static byte[] ConvertBase64ToImage(string base64)
    {
        return Convert.FromBase64String(base64);
    }
}

