using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models.Interfaces
{
    public interface IDataAccessObject
    {
        [PrimaryKey]
        public int ID { get; set; }

        public Task<int> SaveAsync();
        public Task<bool> DeleteAsync();

        public static virtual string SelectAllQuery { get; }
        public static virtual string InsertQuery { get; }
        public static virtual string InsertQueryNoId { get; }
        public static virtual string DeleteQuery { get; }

        public static abstract IDataAccessObject LoadFromRow(DataRow row);
    }
}
