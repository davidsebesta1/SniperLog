using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace SniperLog.Models
{
    public interface IDataAccessObject<T> where T : IDataAccessObject<T>
    {
        public Task<bool> SaveAsync();
        public Task<bool> DeleteAsync();

        public static abstract Task<T?> LoadAsync(int id);
        public static abstract Task<ObservableCollection<T>> LoadAllAsync();
        public static abstract Task<ObservableCollection<T>> LoadAllAsync(ObservableCollection<T> collection);
        public static abstract T LoadNextFromReader(SqliteDataReader reader);
    }
}