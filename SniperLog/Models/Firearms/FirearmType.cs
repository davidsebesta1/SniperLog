using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models;

/// <summary>
/// A class representing the type of the firearm.
/// </summary>
public partial class FirearmType : IDataAccessObject, ICsvProcessable, IEquatable<FirearmType?>
{
    #region Properties

    /// <inheritdoc/>
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// Name of the firearm type.
    /// </summary>
    public string TypeName { get; set; }

    /// <inheritdoc/>
    public static string CsvHeader => "TypeName";

    #endregion

    #region Constructors

    /// <summary>
    /// Primary ctor.
    /// </summary>
    public FirearmType(int iD, string name)
    {
        ID = iD;
        TypeName = name;
    }

    /// <summary>
    /// ID-less ctor.
    /// </summary>
    public FirearmType(string name) : this(-1, name)
    {

    }

    #endregion

    #region DAO Methods

    /// <inheritdoc/>
    public static IDataAccessObject LoadFromRow(DataRow row)
    {
        return new FirearmType(row);
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
            ServicesHelper.GetService<DataCacherService<FirearmType>>().AddOrUpdate(this);
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
            ServicesHelper.GetService<DataCacherService<FirearmType>>().Remove(this);
        }
    }


    #endregion

    #region CSV

    /// <inheritdoc/>
    public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
    {
        return new FirearmType(row);
    }

    /// <inheritdoc/>
    public string SerializeToCsvRow()
    {
        return TypeName;
    }


    #endregion

    #region Object

    /// <inheritdoc/>
    public override string ToString()
    {
        return TypeName;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as FirearmType);
    }

    /// <inheritdoc/>
    public bool Equals(FirearmType? other)
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
    public static bool operator ==(FirearmType? left, FirearmType? right)
    {
        return EqualityComparer<FirearmType>.Default.Equals(left, right);
    }

    /// <inheritdoc/>
    public static bool operator !=(FirearmType? left, FirearmType? right)
    {
        return !(left == right);
    }

    #endregion
}
