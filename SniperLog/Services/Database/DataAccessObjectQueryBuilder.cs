using SniperLog.Extensions;
using SniperLog.Extensions.Pools;
using SniperLog.Models;
using SniperLog.Services.Database.Attributes;
using System.Reflection;
using System.Text;

namespace SniperLog.Services.Database
{
    public static class DataAccessObjectQueryBuilder
    {
        public const int InitialSize = 2;

        private static readonly Dictionary<Type, string> _cachedInsertQueries = new Dictionary<Type, string>(InitialSize);
        private static readonly Dictionary<Type, string> _cachedInsertNoIdQueries = new Dictionary<Type, string>(InitialSize);
        private static readonly Dictionary<Type, string> _cachedDeleteQueries = new Dictionary<Type, string>(InitialSize);
        private static readonly Dictionary<Type, string> _cachedDefSelectQueries = new Dictionary<Type, string>(InitialSize);

        public static string GetInsertQuery(Type type, bool includePK = false, bool includeReturning = true, bool insertOrUpdate = false)
        {
            if (!includePK)
            {
                if (_cachedInsertNoIdQueries.TryGetValue(type, out string val))
                {
                    return val;
                }
            }
            else
            {
                if (_cachedInsertQueries.TryGetValue(type, out string val))
                {
                    return val;
                }
            }
            StringBuilder strBuilder = new StringBuilder(512);

            string className = type.Name;
            IEnumerable<PropertyInfo?> allProperties = type.GetProperties().Where(n => n.MemberType is MemberTypes.Property && !n.IsStatic() && n.CanWrite && !Attribute.IsDefined(n, typeof(DatabaseIgnore)));

            if (!allProperties.Any())
            {
                throw new ArgumentException($"Unable to generate query for class {type.Name} with 0 properties");
            }

            PropertyInfo? primaryKeyProperty = allProperties.FirstOrDefault(n => Attribute.IsDefined(n, typeof(PrimaryKey)));
            if (includeReturning && primaryKeyProperty is null)
            {
                throw new ArgumentException($"Unable to generate query with returning statement for class {type.Name} with Undefined Primary Key Property");
            }

            strBuilder.Append($"INSERT{(insertOrUpdate ? " OR REPLACE " : " ")}INTO ");
            strBuilder.Append(className);
            strBuilder.Append('(');

            PropertyInfo? last = allProperties.Last();
            foreach (PropertyInfo? property in allProperties)
            {
                if (!includePK && property == primaryKeyProperty) continue;

                strBuilder.Append(property.Name);
                if (property != last) strBuilder.Append(',');
            }

            strBuilder.Append(") VALUES(");
            foreach (PropertyInfo? property in allProperties)
            {
                if (!includePK && property == primaryKeyProperty) continue;

                strBuilder.Append($"@{property.Name}");
                if (property != last) strBuilder.Append(',');
            }

            strBuilder.Append(')');

            if (includeReturning)
            {
                strBuilder.Append(" RETURNING ");
                strBuilder.Append(className);
                strBuilder.Append('.');
                strBuilder.Append(primaryKeyProperty.Name);
            }

            strBuilder.Append(';');

            string query = strBuilder.ToString();

            if (!includePK) _cachedInsertNoIdQueries.Add(type, query);
            else _cachedInsertQueries.Add(type, query);
            return query;
        }

        public static string GetInsertQuery<T>(bool includePK = false, bool includeReturning = true, bool insertOrUpdate = false)
        {
            return GetInsertQuery(typeof(T), includePK, includeReturning, insertOrUpdate);
        }

        public static string GetDeleteQuery(Type type)
        {
            if (_cachedDeleteQueries.TryGetValue(type, out string val))
            {
                return val;
            }

            StringBuilder strBuilder = new StringBuilder(512);

            string className = type.Name;

            PropertyInfo? primaryKeyProperty = type.GetProperties().Where(n => n.MemberType is MemberTypes.Property && !n.IsStatic() && n.CanWrite && !Attribute.IsDefined(n, typeof(DatabaseIgnore))).FirstOrDefault(n => Attribute.IsDefined(n, typeof(PrimaryKey)));
            if (primaryKeyProperty is null)
            {
                throw new ArgumentException($"Unable to generate delete query for class {type.Name} with Undefined Primary Key Property");
            }

            strBuilder.Append("DELETE FROM ");
            strBuilder.Append(className);
            strBuilder.Append(" WHERE ");
            strBuilder.Append(className);
            strBuilder.Append('.');
            strBuilder.Append(primaryKeyProperty.Name);
            strBuilder.Append(" = @ID");

            string query = strBuilder.ToString();
            _cachedDeleteQueries.Add(type, query);
            return query;
        }

        public static string GetDeleteQuery<T>()
        {
            return GetDeleteQuery(typeof(T));
        }

        public static string GetSelectQuery<T>(params string[] names)
        {
            return GetSelectQuery(typeof(T), names);
        }

        public static string GetSelectQuery(Type type, params string[] names)
        {
            if (_cachedDefSelectQueries.TryGetValue(type, out string val))
            {
                return val;
            }

            StringBuilder strBuilder = new StringBuilder(512);

            string className = type.Name;
            IEnumerable<PropertyInfo?> allProperties = type.GetProperties().Where(n => n.MemberType is MemberTypes.Property && !n.IsStatic() && n.CanWrite && !Attribute.IsDefined(n, typeof(DatabaseIgnore)));

            if (!allProperties.Any())
            {
                throw new ArgumentException($"Unable to generate select query for class {type.Name} with 0 properties");
            }

            bool selectAll = names.Length == 0;
            if (names.Length == allProperties.Count())
            {
                selectAll = names.All(n => allProperties.Any(m => m?.Name == n));
            }

            strBuilder.Append("SELECT ");
            if (selectAll)
            {
                strBuilder.Append(" * ");
            }
            else
            {
                PropertyInfo? last = allProperties.Last();
                foreach (PropertyInfo? property in allProperties)
                {
                    strBuilder.Append(property.DeclaringType.Name);
                    strBuilder.Append('.');
                    strBuilder.Append(property.Name);
                    if (property != last)
                    {
                        strBuilder.Append(',');
                    }
                }
            }

            strBuilder.Append(" FROM ");
            strBuilder.Append(className);
            strBuilder.Append(';');

            string query = strBuilder.ToString();
            _cachedDefSelectQueries.Add(type, query);
            return query;
        }
    }
}
