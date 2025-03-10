using System.Data;

namespace SniperLog.Extensions;

/// <summary>
/// Static extension class for <see cref="DataRow"/>.
/// </summary>
public static class DataRowExtensions
{
    /// <summary>
    /// Converts row column name to specified type.
    /// </summary>
    /// <typeparam name="T">Generic type to convert the column to.</typeparam>
    /// <param name="row">This row.</param>
    /// <param name="columnName">Name of the column to be converted.</param>
    /// <returns>Generic type or its default.</returns>
    public static T? GetConverted<T>(this DataRow row, string columnName)
    {
        Type? nullableType = Nullable.GetUnderlyingType(typeof(T));
        if (nullableType != null)
        {
            if (row.IsNull(columnName))
                return default;

            object value = Convert.ChangeType(row[columnName], nullableType);
            return (T?)value;
        }

        if (row.IsNull(columnName))
            return default;

        return (T?)Convert.ChangeType(row[columnName], typeof(T));
    }
}
