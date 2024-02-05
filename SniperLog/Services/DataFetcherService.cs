using SniperLog.Models;
using System.Collections.ObjectModel;

namespace SniperLog.Services
{
    public class DataFetcherService<T> where T : IDataAccessObject<T>
    {
        private ObservableCollection<T>? _cachedObservableCollection;

        public async Task<ObservableCollection<T>> GetAll()
        {
            if (_cachedObservableCollection == null)
            {
                _cachedObservableCollection = await T.LoadAllAsync();
                return _cachedObservableCollection;
            }

            return await T.LoadAllAsync(_cachedObservableCollection);
        }
    }
}