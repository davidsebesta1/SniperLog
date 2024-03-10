using SniperLog.Services.Database.Attributes;
using System.Data;
using System.Reflection;

namespace SniperLog.Models
{
    public interface IDataAccessObject
    {
        [PrimaryKey]
        public int ID { get; set; }

        public Task<int> SaveAsync();
        public Task<bool> DeleteAsync();

        public static abstract string SelectAllQuery { get; }
        public static abstract string InsertQuery { get; }
        public static abstract string InsertQueryNoId { get; }
        public static abstract string DeleteQuery { get; }

        public static abstract IDataAccessObject LoadFromRow(DataRow row);
    }
}