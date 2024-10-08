﻿using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class Country : IDataAccessObject, ICsvProcessable, IEquatable<Country?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public static string CsvHeader => "Code,Name";

        #endregion

        #region Constructors

        public Country(int iD, string name, string code)
        {
            ID = iD;
            Name = name;
            Code = code;
        }

        public Country(string name, string code) : this(-1, name, code)
        {

        }

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new Country(row);
        }

        public static async Task<ICsvProcessable> DeserializeFromCsvRow(string row)
        {
            string[] data = row.Split(',');
            return new Country(data[1], data[0]);
        }

        public string SerializeToCsvRow()
        {
            return $"{Code},{Name}";
        }

        #endregion

        #region DAO Methods

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
                ServicesHelper.GetService<DataCacherService<Country>>().AddOrUpdate(this);
            }
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<Country>>().Remove(this);
            }
        }

        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as Country);
        }

        public bool Equals(Country? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Country? left, Country? right)
        {
            return EqualityComparer<Country>.Default.Equals(left, right);
        }

        public static bool operator !=(Country? left, Country? right)
        {
            return !(left == right);
        }

        #endregion

        #region Other

        public override string? ToString()
        {
            return Name;
        }

        #endregion
    }
}
