using System.Data;

namespace SniperLog.Models.Interfaces
{
    /// <summary>
    /// Base interface providing features for Data Access Object pattern
    /// </summary>
    public interface IDataAccessObject
    {
        /// <summary>
        /// Primary key for any DAO object
        /// </summary>
        [PrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// Asynchronous method for saving the object in database
        /// </summary>
        /// <returns>ID of the object</returns>
        public Task<int> SaveAsync();

        /// <summary>
        /// Asynchronous method for deleting the object from database
        /// </summary>
        /// <returns>Boolean value whenever the deletion was successful</returns>
        public Task<bool> DeleteAsync();

        /// <summary>
        /// Query for returning all object of this type
        /// </summary>
        public static virtual string SelectAllQuery { get; }

        /// <summary>
        /// Query for updating already existing object into database
        /// </summary>
        public static virtual string InsertQuery { get; }

        /// <summary>
        /// Query for inserting a new object into database. Returns object´s internal database ID.
        /// </summary>
        public static virtual string InsertQueryNoId { get; }

        /// <summary>
        /// Query for deleteing object from database
        /// </summary>
        public static virtual string DeleteQuery { get; }

        /// <summary>
        /// Method for loading this object from a standard DataRow object
        /// </summary>
        /// <param name="row"></param>
        /// <returns>A new object casted to this interface</returns>
        public static abstract IDataAccessObject LoadFromRow(DataRow row);
    }
}
