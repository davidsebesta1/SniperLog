﻿using CommunityToolkit.Mvvm.Input;
using SniperLog.Config;
using SniperLog.Extensions;
using SniperLog.Services.Serialization;

namespace SniperLog.ViewModels.Other;

/// <summary>
/// View model to initially setup the database.
/// </summary>
public partial class InitialSetupPopupPageViewModel : BaseViewModel
{
    private readonly CsvProcessor _csvProcessor;

    /// <summary>
    /// Ctor.
    /// </summary>
    public InitialSetupPopupPageViewModel(CsvProcessor processor) : base()
    {
        _csvProcessor = processor;
    }

    /// <summary>
    /// Loads initial database query.
    /// </summary>
    [RelayCommand]
    private async Task LoadInitialDatabase()
    {
        await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(await MauiExtensions.ReadTextFileAsync("SETUPQUERY.sql"));
    }

    /// <summary>
    /// Loads initial data such as countries, sight click types, calibers and manufacturer types.
    /// <para>Also adds initial db values for debug builds.</para>
    /// </summary>
    [RelayCommand]
    private async Task LoadInitialData()
    {
        await _csvProcessor.LoadToDatabase<Country>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("Countries.csv")));
        await _csvProcessor.LoadToDatabase<SightClickType>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("SightClickTypes.csv")));
        await _csvProcessor.LoadToDatabase<FirearmType>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("FirearmTypes.csv")));
        await _csvProcessor.LoadToDatabase<FirearmCaliber>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("FirearmCalibers.csv")));
        await _csvProcessor.LoadToDatabase<ManufacturerType>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("ManufacturerTypes.csv")));

#if DEBUG
        //Shooting ranges
        ShootingRange s1 = new ShootingRange("Oleško", "Oleško, Litoměřice", 50.486238d, 14.201412d, false);
        ShootingRange s2 = new ShootingRange("TestFav", "Fav", 1d, 2d, true);

        await s1.SaveAsync();
        await s2.SaveAsync();

        //Subranges
        SubRange testSub = new SubRange(s1.ID, 300, 123, 46, 0, 'B');
        await testSub.SaveAsync();
        await testSub.SaveNotesAsync("test notes");

        //Manufacturers
        Manufacturer manufacturer = new Manufacturer(15, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(static n => n.Name == "Firearm")).ID, "TestMan");
        Manufacturer manufacturer2 = new Manufacturer(16, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(static n => n.Name == "Firearm")).ID, "Ceska Zbrojovka");

        await manufacturer.SaveAsync();
        await manufacturer2.SaveAsync();

        Manufacturer manufacturer3 = new Manufacturer(14, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(static n => n.Name == "Sight")).ID, "Vortex");
        Manufacturer manufacturer4 = new Manufacturer(13, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(static n => n.Name == "Sight")).ID, "Morkite");

        await manufacturer3.SaveAsync();
        await manufacturer4.SaveAsync();

        Manufacturer manufacturer5 = new Manufacturer(14, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(static n => n.Name == "Bullet")).ID, "Lapua");

        await manufacturer5.SaveAsync();

        Manufacturer manufacturer6 = new Manufacturer(14, (await ServicesHelper.GetService<DataCacherService<ManufacturerType>>().GetFirstBy(static n => n.Name == "Ammunition")).ID, "Lapua");

        await manufacturer6.SaveAsync();

        //Reticles
        SightReticle reticle1 = new SightReticle("TestRet");
        SightReticle reticle2 = new SightReticle("TreeReticle");

        await reticle1.SaveAsync();
        await reticle2.SaveAsync();

        //Sights
        FirearmSight sight1 = new FirearmSight((await ServicesHelper.GetService<DataCacherService<SightClickType>>().GetFirstBy(static n => n.ClickTypeName == "MRAD")).ID, 3, reticle1.ID, "Vortex1", 0.1d);
        FirearmSight sight2 = new FirearmSight((await ServicesHelper.GetService<DataCacherService<SightClickType>>().GetFirstBy(static n => n.ClickTypeName == "MOA")).ID, 4, reticle2.ID, "MOAScope", 0.25d);

        await sight1.SaveAsync();
        await sight2.SaveAsync();

        FirearmSightSetting set1 = new FirearmSightSetting(sight1.ID, 100, 0, -1);
        FirearmSightSetting set2 = new FirearmSightSetting(sight1.ID, 150, 3, -1);
        FirearmSightSetting set3 = new FirearmSightSetting(sight1.ID, 200, 8, 0);

        await set1.SaveAsync();
        await set2.SaveAsync();
        await set3.SaveAsync();

        Firearm firearm1 = new Firearm(1, 2, 3, sight1.ID, "Tikka", null, null, 1200, 17, "1:8", 8, false, 3d);
        Firearm firearm2 = new Firearm(2, 1, 15, sight2.ID, "AR-15", "Gen1", "VGDH366V3-D", 800, 13, "1:8", 4, false, 3.5d);

        await firearm1.SaveAsync();
        await firearm2.SaveAsync();

        Bullet bullet = new Bullet((await ServicesHelper.GetService<DataCacherService<FirearmCaliber>>().GetFirstBy(static n => n.Caliber.Contains(".308"))).ID, manufacturer5.ID, 13.6d, 13, 34, 0.3d, 0.7d);
        await bullet.SaveAsync();

        Ammunition ammunition = new Ammunition(bullet.ID, 120, 2.2d);
        await ammunition.SaveAsync();

        MuzzleVelocity muzzleVelocity = new MuzzleVelocity(ammunition.ID, firearm1.ID, 380d);
        muzzleVelocity.SaveAsync();
#endif
    }

    /// <summary>
    /// Finalizes initial load by marking application first launch status to false.
    /// </summary>
    [RelayCommand]
    private void FinalizeInitialLoad()
    {
        VersionControl tracking = ApplicationConfigService.GetConfig<VersionControl>();
        tracking.FirstLaunchEver = false;
        ApplicationConfigService.SaveConfig(tracking);
    }
}
