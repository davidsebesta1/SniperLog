using Microsoft.Data.Sqlite;
using SniperLog.Config;
using System.Data;

namespace SniperLog.Models;

/// <summary>
/// A class representing a firearm.
/// </summary>
public partial class Firearm : ObservableObject, IDataAccessObject, INoteSaveable, IEquatable<Firearm?>
{
    #region Properties

    /// <inheritdoc/>
    public string NotesSavePath
    {
        get
        {
            return Path.Combine("Data", "Firearms", Name, "notes.txt");
        }
    }

    /// <inheritdoc/>
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// ID of the referenced firearm type.
    /// </summary>
    [ForeignKey(typeof(FirearmType), nameof(FirearmType.ID))]
    [ObservableProperty]
    private int _firearmType_ID;

    /// <summary>
    /// ID of the referenced manufacturer
    /// </summary>
    [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
    [ObservableProperty]
    private int? _manufacturer_ID;

    /// <summary>
    /// ID of the referenced caliber.
    /// </summary>
    [ForeignKey(typeof(FirearmCaliber), nameof(FirearmCaliber.ID))]
    [ObservableProperty]
    private int _caliber_ID;

    /// <summary>
    /// ID of the referenced firearm sight.
    /// </summary>
    [ForeignKey(typeof(FirearmSight), nameof(FirearmSight.ID))]
    [ObservableProperty]
    private int _firearmSight_ID;

    /// <summary>
    /// Firearm name.
    /// </summary>
    [ObservableProperty]
    public string _name;

    /// <summary>
    /// Firearm's model.
    /// </summary>
    [ObservableProperty]
    private string? _model;

    /// <summary>
    /// Serial number of the firearm.
    /// </summary>
    [ObservableProperty]
    private string? _serialNumber;

    /// <summary>
    /// Total length in milimeters of the firearm.
    /// </summary>
    [ObservableProperty]
    private double? _totalLengthMm;

    /// <summary>
    /// Barel length of the firearm in inches.
    /// </summary>
    [ObservableProperty]
    private double? _barrelLengthInch;
    
    /// <summary>
    /// Rate of twist of the firearm.
    /// </summary>
    [ObservableProperty]
    private string? _rateOfTwist;

    /// <summary>
    /// Weight of the firearm in kilos.
    /// </summary>
    [ObservableProperty]
    private double? _weight;

    /// <summary>
    /// Whether the firearm is made for left-handed people.
    /// </summary>
    [ObservableProperty]
    private bool? _handednessForLeft;
    
    /// <summary>
    /// Vertical offset of the firearm sight and barrel in cm.
    /// </summary>
    [ObservableProperty]
    private double? _sightHeightCm;

    #endregion

    #region Constructors

    /// <summary>
    /// Primary ctor.
    /// </summary>
    public Firearm(int iD, int firearmType_ID, int? manufacturer_ID, int caliber_ID, int sight_ID, string name, string? model, string? serialNumber, double? totalLengthMm, double? barrelLengthInch, string? rateOfTwist, double? weight, bool? handednessForLeft, double? sightHeightCm)
    {
        ID = iD;
        FirearmType_ID = firearmType_ID;
        Manufacturer_ID = manufacturer_ID;
        Caliber_ID = caliber_ID;
        FirearmSight_ID = sight_ID;
        Name = name;
        Model = model;
        SerialNumber = serialNumber;
        TotalLengthMm = totalLengthMm;
        BarrelLengthInch = barrelLengthInch;
        RateOfTwist = rateOfTwist;
        Weight = weight;
        HandednessForLeft = handednessForLeft;
        SightHeightCm = sightHeightCm;
    }

    /// <summary>
    /// ID-less ctor.
    /// </summary>
    public Firearm(int firearmType_ID, int? manufacturer_ID, int caliber_ID, int sight_ID, string name, string? model, string? serialNumber, double? totalLengthMm, double? barrelLengthInch, string? rateOfTwist, double? weight, bool? handednessForLeft, double? sightHeightCm) : this(-1, firearmType_ID, manufacturer_ID, caliber_ID, sight_ID, name, model, serialNumber, totalLengthMm, barrelLengthInch, rateOfTwist, weight, handednessForLeft, sightHeightCm) { }

    #endregion

    #region DAO Methods

    /// <inheritdoc/>
    public static IDataAccessObject LoadFromRow(DataRow row)
    {
        return new Firearm(row);
    }

    /// <inheritdoc/>
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
            ServicesHelper.GetService<DataCacherService<Firearm>>().AddOrUpdate(this);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync()
    {
        try
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
        }
        finally
        {
            ServicesHelper.GetService<DataCacherService<Firearm>>().Remove(this);
            SetLastSelectedAmmunition(null);
            DeleteNotes();
        }
    }

    /// <summary>
    /// Gets the last selected ammunition for this firearm for recording entries.
    /// </summary>
    /// <returns>Ammunition that was lastly selected or null.</returns>
    public async Task<Ammunition?> GetLastSelectedAmmunition() => await ApplicationConfigService.GetConfig<PreferencesConfig>().GetLastSelectedAmmunition(this);

    /// <summary>
    /// Sets the last selected ammunition for this firearm and saves the preferences.
    /// </summary>
    /// <param name="ammo">The ammunition that has been selected as last.</param>
    public void SetLastSelectedAmmunition(Ammunition? ammo) => ApplicationConfigService.GetConfig<PreferencesConfig>().SetLastSelectedAmmunition(this, ammo);

    #endregion

    #region Object

    /// <inheritdoc/>
    public override string ToString()
    {
        return Name;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Firearm);
    }

    /// <inheritdoc/>
    public bool Equals(Firearm? other)
    {
        return other is not null &&
               ID == other.ID;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <inheritdoc/>
    public static bool operator ==(Firearm? left, Firearm? right)
    {
        return left is Firearm f1 && right is Firearm f2 && f1.Equals(f2);
    }

    /// <inheritdoc/>
    public static bool operator !=(Firearm? left, Firearm? right)
    {
        return !(left == right);
    }

    #endregion
}

