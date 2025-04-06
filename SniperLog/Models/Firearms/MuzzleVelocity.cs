using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models;

/// <summary>
/// A class represrnting muzzle velocity.
/// </summary>
public partial class MuzzleVelocity : ObservableObject, IDataAccessObject, IEquatable<MuzzleVelocity?>
{
    /// <inheritdoc/>
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// ID of the referenced ammunition.
    /// </summary>
    [ForeignKey(typeof(Ammunition), nameof(Ammunition.ID))]
    [ObservableProperty]
    private int _ammo_ID;

    /// <summary>
    /// ID of the referenced firearm.
    /// </summary>
    [ForeignKey(typeof(Firearm), nameof(Firearm.ID))]
    [ObservableProperty]
    private int _firearm_ID;

    /// <summary>
    /// Muzzle velocity in meters per second.
    /// </summary>
    [ObservableProperty]
    private double _velocityMS;

    /// <summary>
    /// Primary ctor.
    /// </summary>
    public MuzzleVelocity(int iD, int ammoID, int firearmID, double velocityMS)
    {
        ID = iD;
        Ammo_ID = ammoID;
        Firearm_ID = firearmID;
        VelocityMS = velocityMS;
    }

    /// <summary>
    /// ID-less ctor.
    /// </summary>
    public MuzzleVelocity(int ammoID, int firearmID, double velocityMS) : this(-1, ammoID, firearmID, velocityMS)
    {

    }

    /// <inheritdoc/>
    public static IDataAccessObject LoadFromRow(DataRow row)
    {
        return new MuzzleVelocity(row);
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
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery,
                   GetSqliteParams(true));
        }
        finally
        {
            ServicesHelper.GetService<DataCacherService<MuzzleVelocity>>().AddOrUpdate(this);
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
            ServicesHelper.GetService<DataCacherService<MuzzleVelocity>>().Remove(this);
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as MuzzleVelocity);
    }

    /// <inheritdoc/>
    public bool Equals(MuzzleVelocity? other)
    {
        return other is not null &&
               ID == other.ID;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ID, VelocityMS);
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        return VelocityMS + " m/s";
    }
}
