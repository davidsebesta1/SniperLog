using System.Data;

namespace SniperLog.Extensions
{
    public static class DataRowExtensions
    {
        public static T? GetConverted<T>(this DataRow row, string columnName)
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal))
            {
                return (T)Convert.ChangeType(row[columnName], typeof(T));
            }

            throw new ArgumentException("T must be a primitive data type, string, or decimal.");
        }
    }
}