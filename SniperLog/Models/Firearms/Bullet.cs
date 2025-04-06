using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models;

/// <summary>
/// A class representing a bullet.
/// </summary>
public partial class Bullet : ObservableObject, IDataAccessObject, IEquatable<Bullet?>
{
    /// <inheritdoc/>
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// ID of the refenreced caliber.
    /// </summary>
    [ForeignKey(typeof(FirearmCaliber), nameof(FirearmCaliber.ID))]
    [ObservableProperty]
    private int _caliber_ID;

    /// <summary>
    /// ID of the referenced manufacturer.
    /// </summary>
    [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
    [ObservableProperty]
    private int _manufacturer_ID;

    /// <summary>
    /// Wight of the bullet in grams.
    /// </summary>
    [ObservableProperty]
    private double _weightGrams;

    /// <summary>
    /// Diamater of the bullet.
    /// </summary>
    [ObservableProperty]
    private double _bulletDiameter;

    /// <summary>
    /// Length of the bullet in mm.
    /// </summary>
    [ObservableProperty]
    private double _bulletLength;

    /// <summary>
    /// Ballistic coeficient 1 of the bullet.
    /// </summary>
    [ObservableProperty]
    private double? _bCG1;

    /// <summary>
    /// Ballistic coeficient 7 of the bullet.
    /// </summary>
    [ObservableProperty]
    private double? _bCG7;

    /// <summary>
    /// Primary ctor.
    /// </summary>
    public Bullet(int iD, int caliberID, int manuId, double weightGrams, double bulletdia, double bulletlen, double? bc1, double? bc7)
    {
        ID = iD;
        Caliber_ID = caliberID;
        Manufacturer_ID = manuId;
        WeightGrams = weightGrams;
        BulletDiameter = bulletdia;
        BulletLength = bulletlen;
        BCG1 = bc1;
        BCG7 = bc7;
    }

    /// <summary>
    /// ID-less ctor.
    /// </summary>
    public Bullet(int caliberID, int manuId, double weightGrams, double bulletdia, double bulletlen, double? bc1, double? bc7) : this(-1, caliberID, manuId, weightGrams, bulletdia, bulletlen, bc1, bc7)
    {

    }

    /// <inheritdoc/>
    public static IDataAccessObject LoadFromRow(DataRow row)
    {
        return new Bullet(row);
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
            ServicesHelper.GetService<DataCacherService<Bullet>>().AddOrUpdate(this);
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
            ServicesHelper.GetService<DataCacherService<Bullet>>().Remove(this);
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Bullet);
    }

    /// <inheritdoc/>
    public bool Equals(Bullet? other)
    {
        return other is not null &&
               ID == other.ID;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ID, WeightGrams, BCG1, BCG7);
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        return WeightGrams + "g, BC1: " + BCG1 + ", BC7: " + BCG7;
    }
}

