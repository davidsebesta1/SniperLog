using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Models
{
    public interface IDataAccessObject<T> where T : IDataAccessObject<T>
    {
        public Task<int> SaveAsync();
        public Task<bool> DeleteAsync();

        public static abstract Task<T?> LoadNewAsync(int id);
        public static abstract T? GetById(int id);
        public static abstract Task<ObservableCollection<T>> LoadAllAsync();
        public static abstract Task<ObservableCollection<T>> LoadAllAsync(ObservableCollection<T> collection);
        public static abstract T LoadFromRow(DataRow reader);
    }
}