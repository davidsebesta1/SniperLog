using SniperLog.Models;
using SniperLog.Services.Database;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace SniperLog.Services
{
    public class DataCacherService<T> where T : IDataAccessObject
    {
        private readonly ConcurrentDictionary<int, T> _cachedPerId = new ConcurrentDictionary<int, T>();
        private readonly ObservableCollection<T> _objects = new ObservableCollection<T>();

        public static event EventHandler<DataServiceOnSaveOrUpdateArgs<T>> OnSaveOrUpdate;
        public static event EventHandler<DataServiceOnDeleteArgs<T>> OnDelete;

        public ObservableCollection<T> GetAll()
        {
            if (_cachedPerId.IsEmpty)
            {
                DataTable table = SqLiteDatabaseConnection.Instance.ExecuteQuery(T.SelectAllQuery);
                foreach (DataRow row in table.Rows)
                {
                    T obj = (T)T.LoadFromRow(row);
                    _cachedPerId.TryAdd(obj.ID, obj);
                    _objects.Add(obj);
                }
            }
            return _objects;
        }

        public async Task<bool> AddOrUpdateAsync(T item)
        {
            if (_cachedPerId.ContainsKey(item.ID))
            {
                await item.SaveAsync();
                OnSaveOrUpdate?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(item, true));
                return false;
            }


            await item.SaveAsync();
            _cachedPerId.TryAdd(item.ID, item);
            _objects.Add(item);
            OnSaveOrUpdate?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(item, false));
            return true;
        }

        public async Task<bool> RemoveAsync(T item)
        {
            await item.DeleteAsync();
            bool res = _cachedPerId.Remove(item.ID, out T val) && _objects.Remove(item);
            OnDelete?.Invoke(this, new DataServiceOnDeleteArgs<T>(item));
            return res;
        }

        public ObservableCollection<T> GetAllBy(Predicate<T> predicate)
        {
            return new ObservableCollection<T>(GetAll().Where(n => predicate(n)));
        }

        public T? GetFirstBy(Predicate<T> predicate)
        {
            return GetAll().FirstOrDefault(n => predicate(n));
        }
    }

    public class DataServiceOnDeleteArgs<T> : EventArgs
    {
        public Type Type;
        public T ItemDeleted;

        public DataServiceOnDeleteArgs(T itemDeleted) : base()
        {
            Type = itemDeleted.GetType();
            ItemDeleted = itemDeleted;
        }
    }

    public class DataServiceOnSaveOrUpdateArgs<T> : EventArgs
    {
        public Type Type;
        public T ItemSaved;
        public bool SavedNewItem;

        public DataServiceOnSaveOrUpdateArgs(T itemSaved, bool isNewItem) : base()
        {
            Type = itemSaved.GetType();
            ItemSaved = itemSaved;
            SavedNewItem = isNewItem;
        }
    }
}