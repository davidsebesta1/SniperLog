using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models;

/// <summary>
/// A class representing firearm ammunition.
/// </summary>
public partial class Ammunition : ObservableObject, IDataAccessObject, IEquatable<Ammunition?>
{
    /// <inheritdoc/>
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// ID of the referenced bullet.
    /// </summary>
    [ForeignKey(typeof(Bullet), nameof(Bullet.ID))]
    [ObservableProperty]
    private int _bullet_ID;

    /// <summary>
    /// Total length of the ammunition in milimeters.
    /// </summary>
    [ObservableProperty]
    private double _totalLengthMm;

    /// <summary>
    /// Total weight of the gunpowder used in grams.
    /// </summary>
    [ObservableProperty]
    private double _gunpowderAmountGrams;

    /// <summary>
    /// Primary constructor.
    /// </summary>
    public Ammunition(int iD, int bulletID, double totalLengthMm, double gunpowderAmountGrams)
    {
        ID = iD;
        Bullet_ID = bulletID;
        TotalLengthMm = totalLengthMm;
        GunpowderAmountGrams = gunpowderAmountGrams;
    }

    /// <summary>
    /// ID-less constructor.
    /// </summary>
    public Ammunition(int bulletID, double totalLengthMm, double gunpowderAmountGrams) : this(-1, bulletID, totalLengthMm, gunpowderAmountGrams)
    {

    }

    /// <inheritdoc/>
    public static IDataAccessObject LoadFromRow(DataRow row)
    {
        return new Ammunition(row);
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
            ServicesHelper.GetService<DataCacherService<Ammunition>>().AddOrUpdate(this);
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
            ServicesHelper.GetService<DataCacherService<Ammunition>>().Remove(this);
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Ammunition);
    }

    /// <inheritdoc/>
    public bool Equals(Ammunition? other)
    {
        return other is not null &&
               ID == other.ID;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ID, TotalLengthMm, GunpowderAmountGrams);
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        Bullet bullet = ReferencedBullet;
        return $"{bullet.ReferencedManufacturer.Name} {bullet.ReferencedFirearmCaliber.Caliber}";
    }
}

