using System.Data;

namespace SniperLog.Extensions
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// Converts row column name to specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="columnName"></param>st
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T? GetConverted<T>(this DataRow row, string columnName)
        {
            Type? nullableType = Nullable.GetUnderlyingType(typeof(T));
            if (nullableType != null)
            {
                if (row.IsNull(columnName))
                {
                    return default;
                }

                object value = Convert.ChangeType(row[columnName], nullableType);
                return (T?)value;
            }

            if (row.IsNull(columnName))
            {
                return default;
            }

            return (T?)Convert.ChangeType(row[columnName], typeof(T));
        }
    }
}