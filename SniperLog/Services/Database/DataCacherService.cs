using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Services.Database
{
    public class DataCacherService<T> where T : IDataAccessObject
    {
        private readonly ConcurrentDictionary<int, T> _cachedPerId = new ConcurrentDictionary<int, T>();
        private readonly ObservableCollection<T> _objects = new ObservableCollection<T>();

        public static event EventHandler<DataServiceOnSaveOrUpdateArgs<T>> OnSaveOrUpdate;
        public static event EventHandler<DataServiceOnDeleteArgs<T>> OnDelete;
        public static event EventHandler OnChanged;

        public ObservableCollection<T> GetAll()
        {
            if (_cachedPerId.IsEmpty)
            {
                DataTable table = SqLiteDatabaseConnection.Instance.ExecuteQuery(T.SelectAllQuery);

                if (table == null) return _objects;

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
                OnSaveOrUpdate?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(item, true));
                OnChanged?.Invoke(this, null);
                return false;
            }


            _cachedPerId.TryAdd(item.ID, item);
            _objects.Add(item);
            OnSaveOrUpdate?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(item, false));
            OnChanged?.Invoke(this, null);
            return true;
        }

        public async Task<bool> RemoveAsync(T item)
        {
            bool res = _cachedPerId.Remove(item.ID, out var val) && _objects.Remove(item);
            OnDelete?.Invoke(this, new DataServiceOnDeleteArgs<T>(item));
            OnChanged?.Invoke(this, null);
            return res;
        }

        public ObservableCollection<T> GetAllBy(Predicate<T> predicate)
        {
            return new ObservableCollection<T>(GetAll().Where(n => predicate(n)));
        }

        public T? GetFirstBy(Predicate<T> predicate)
        {
            if (!_objects.Any()) GetAll();
            return _objects.FirstOrDefault(n => predicate(n));
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