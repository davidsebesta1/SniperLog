using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models;

/// <summary>
/// A class representing firearm caliber.
/// </summary>
public partial class FirearmCaliber : IDataAccessObject, ICsvProcessable, IEquatable<FirearmCaliber?>
{
    #region Properties

    /// <inheritdoc/>
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// Caliber name.
    /// </summary>
    public string Caliber { get; set; }

    /// <inheritdoc/>
    public static string CsvHeader => "Name";

    #endregion

    #region Constructors

    /// <summary>
    /// Primary ctor.
    /// </summary>
    public FirearmCaliber(int iD, string name)
    {
        ID = iD;
        Caliber = name;
    }

    /// <summary>
    /// ID-less ctor.
    /// </summary>
    public FirearmCaliber(string name) : this(-1, name)
    {

    }

    #endregion

    #region DAO Methods

    /// <inheritdoc/>
    public static IDataAccessObject LoadFromRow(DataRow row)
    {
        return new FirearmCaliber(row);
    }

    /// <inheritdoc/>
    public async Task<int> SaveAsync()
    {
        try
        {
            if (ID == -1)
            {
                ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId, GetSqliteParams(true));
                return ID;
            }
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery, GetSqliteParams(false));
        }
        finally
        {
            ServicesHelper.GetService<DataCacherService<FirearmCaliber>>().AddOrUpdate(this);
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
            ServicesHelper.GetService<DataCacherService<FirearmCaliber>>().Remove(this);
        }
    }

    #endregion

    #region CSV

    /// <inheritdoc/>
    public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
    {
        return new FirearmCaliber(row);
    }

    /// <inheritdoc/>
    public string SerializeToCsvRow()
    {
        return Caliber;
    }

    #endregion

    #region Object

    /// <inheritdoc/>
    public override string ToString()
    {
        return Caliber;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as FirearmCaliber);
    }

    /// <inheritdoc/>
    public bool Equals(FirearmCaliber? other)
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
    public static bool operator ==(FirearmCaliber? left, FirearmCaliber? right)
    {
        return EqualityComparer<FirearmCaliber>.Default.Equals(left, right);
    }

    /// <inheritdoc/>
    public static bool operator !=(FirearmCaliber? left, FirearmCaliber? right)
    {
        return !(left == right);
    }

    #endregion
}
