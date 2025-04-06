using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data;

namespace SniperLog.Services.Database;

/// <summary>
/// Data cacher service for caching database objects in order to speed up the application.
/// </summary>
/// <typeparam name="T">Typeof the model to use.</typeparam>
public class DataCacherService<T> where T : IDataAccessObject
{
    private readonly ConcurrentDictionary<int, T> _cachedPerId = new ConcurrentDictionary<int, T>();
    private readonly ObservableCollection<T> _objects = new ObservableCollection<T>();

    /// <summary>
    /// Event fired whenever any save or update is executed.
    /// </summary>
    public event EventHandler<DataServiceOnSaveOrUpdateArgs<T>> OnSaveOrUpdate;

    /// <summary>
    /// Event fired when any object is deleted from the database.
    /// </summary>
    public event EventHandler<DataServiceOnDeleteArgs<T>> OnDelete;

    /// <summary>
    /// Ctor.
    /// </summary>
    public DataCacherService()
    {

    }

    /// <summary>
    /// Gets all objects of the type.
    /// </summary>
    /// <returns>Cacher observable collection. Not a new instance.</returns>
    public async Task<ObservableCollection<T>> GetAll()
    {
        if (_cachedPerId.IsEmpty)
        {
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync(T.SelectAllQuery);

            if (table == null)
                return _objects;

            foreach (DataRow row in table.Rows)
            {
                T obj = (T)T.LoadFromRow(row);
                _cachedPerId.TryAdd(obj.ID, obj);
                _objects.Add(obj);
            }
        }
        return _objects;
    }

    /// <summary>
    /// Adds or updates item.
    /// </summary>
    /// <param name="item">Item to be added or updated.</param>
    /// <returns>Whether the operation was successful.</returns>
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

    /// <summary>
    /// Removes the item from cache.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item)
    {
        bool res = _cachedPerId.Remove(item.ID, out T? val) && _objects.Remove(item);
        OnDelete?.Invoke(this, new DataServiceOnDeleteArgs<T>(item));
        return res;
    }

    /// <summary>
    /// Gets all objects by predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter with.</param>
    /// <returns>A new collection.</returns>
    public async Task<ObservableCollection<T>> GetAllBy(Predicate<T> predicate)
    {
        return new ObservableCollection<T>((await GetAll()).Where(n => predicate(n)));
    }

    /// <summary>
    /// Gets the first object to match the predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>First matching predicate or null.</returns>
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