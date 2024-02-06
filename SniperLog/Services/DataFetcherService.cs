using SniperLog.Models;
using System.Collections.ObjectModel;

namespace SniperLog.Services
{
    public class DataFetcherService<T> where T : IDataAccessObject<T>
    {
        public async Task GetAll(ObservableCollection<T> collection)
        {
            await T.LoadAllAsync(collection);
        }
    }
}