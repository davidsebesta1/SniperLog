using SniperLog.Extensions.Pools;
using SniperLog.Models;
using SniperLog.Services.Database.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Services.Database
{
    public static class DataAccessObjectInsertQueryBuilder
    {
        public static string GetInsertQueryStatement<T>(bool addReturningId = false, bool insertOrUpdate = false) where T : IDataAccessObject<T>
        {
            return GetInsertQueryStatement<T>(addReturningId, insertOrUpdate, Array.Empty<string>());
        }

        public static string GetInsertQueryStatement<T>(bool addReturningId = false, bool insertOrUpdate = false, params string[] excludedPropertiesNames) where T : IDataAccessObject<T>
        {
            string className = typeof(T).Name;
            IEnumerable<PropertyInfo?> allProperties = typeof(T).GetProperties().Where(n => n.MemberType is MemberTypes.Property && !excludedPropertiesNames.Contains(n.Name) && !Attribute.IsDefined(n, typeof(DatabaseIgnore)));

            if (!allProperties.Any())
            {
                throw new ArgumentException("Unable to generate query for class with 0 properties");
            }

            PropertyInfo? primaryKeyProperty = allProperties.FirstOrDefault(n => Attribute.IsDefined(n, typeof(PrimaryKey)));
            if (addReturningId && primaryKeyProperty == null)
            {
                throw new ArgumentException("Unable to generate query with returning statement for class with Undefined Primary key Property");
            }

            StringBuilder builder = StringBuilderPool.Instance.Rent();
            builder.Append($"INSERT{(insertOrUpdate ? " OR REPLACE " : " ")}INTO ");
            builder.Append(className);
            builder.Append('(');

            PropertyInfo? last = allProperties.Last();
            foreach (PropertyInfo? property in allProperties)
            {
                if (property == primaryKeyProperty) continue;

                builder.Append(property.Name);
                if (property != last) builder.Append(',');
            }

            builder.Append(") VALUES(");
            foreach (PropertyInfo? property in allProperties)
            {
                if (property == primaryKeyProperty) continue;

                builder.Append($"@{property.Name}");
                if (property != last) builder.Append(',');
            }

            builder.Append(')');

            if (addReturningId)
            {
                builder.Append(" RETURNING ");
                builder.Append(className);
                builder.Append('.');
                builder.Append(primaryKeyProperty.Name);
            }

            builder.Append(';');

            return StringBuilderPool.Instance.ReturnToString(builder);
        }
    }
}
