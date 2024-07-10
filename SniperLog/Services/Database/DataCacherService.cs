using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Services.Database
{
    public class DataCacherService<T> where T : IDataAccessObject
    {
        private readonly ConcurrentDictionary<int, T> _cachedPerId = new ConcurrentDictionary<int, T>();
        private readonly ObservableCollection<T> _objects = new ObservableCollection<T>();

        public event EventHandler<DataServiceOnSaveOrUpdateArgs<T>> OnSaveOrUpdate;
        public event EventHandler<DataServiceOnDeleteArgs<T>> OnDelete;

        public DataCacherService()
        {

        }

        public async Task<ObservableCollection<T>> GetAll()
        {
            if (_cachedPerId.IsEmpty)
            {
                DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync(T.SelectAllQuery);

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

        public bool AddOrUpdate(T item)
        {
            if (_cachedPerId.ContainsKey(item.ID))
            {
                OnSaveOrUpdate?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(item, true));
                return false;
            }


            _cachedPerId.TryAdd(item.ID, item);
            _objects.Add(item);
            OnSaveOrUpdate?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(item, false));
            return true;
        }

        public bool Remove(T item)
        {
            bool res = _cachedPerId.Remove(item.ID, out var val) && _objects.Remove(item);
            OnDelete?.Invoke(this, new DataServiceOnDeleteArgs<T>(item));
            return res;
        }

        public async Task<ObservableCollection<T>> GetAllBy(Predicate<T> predicate)
        {
            return new ObservableCollection<T>((await GetAll()).Where(n => predicate(n)));
        }

        public async Task<T?> GetFirstBy(Predicate<T> predicate)
        {
            if (!_objects.Any()) await GetAll();
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