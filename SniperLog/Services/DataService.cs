using SniperLog.Extensions.Pools;
using SniperLog.Models;
using System.Collections.ObjectModel;
using System.Text;

namespace SniperLog.Services
{
    public class DataService<T> where T : IDataAccessObject<T>
    {
        public event EventHandler<DataServiceOnDeleteArgs<T>> OnDelete;
        public event EventHandler<DataServiceOnSaveOrUpdateArgs<T>> OnSave;
        public event EventHandler<DataServiceOnCollectionReSelectedArgs<T>> OnCollectionReSelected;

        public ObservableCollection<T> Data { get; set; } = new ObservableCollection<T>();

        public async Task GetAllToCollection()
        {
            int amountOld = Data.Count;

            await T.LoadAllAsync(Data);

            int amountNew = Data.Count;
            OnCollectionReSelected?.Invoke(this, new DataServiceOnCollectionReSelectedArgs<T>(amountNew - amountOld));
        }

        public async Task SaveOrUpdateAsync(T value)
        {
            await value.SaveAsync();

            bool isNew = false;
            if (!Data.Contains(value))
            {
                Data.Add(value);
                isNew = true;
            }

            OnSave?.Invoke(this, new DataServiceOnSaveOrUpdateArgs<T>(value, isNew));
        }

        public async Task<bool> DeleteAsync(T value)
        {
            await value.DeleteAsync();
            bool res = Data.Remove(value);
            OnDelete?.Invoke(this, new DataServiceOnDeleteArgs<T>(value));

            return res;
        }

        public override string? ToString()
        {
            StringBuilder sb = StringBuilderPool.Instance.Rent();

            sb.AppendLine(GetType().Name);
            foreach (var value in Data)
            {
                sb.AppendLine(sb.ToString());
            }

            return StringBuilderPool.Instance.ReturnToString(sb);
        }
    }

    public class DataServiceOnDeleteArgs<T> : EventArgs
    {
        public T ItemDeleted;

        public DataServiceOnDeleteArgs(T itemDeleted) : base()
        {
            ItemDeleted = itemDeleted;
        }
    }

    public class DataServiceOnSaveOrUpdateArgs<T> : EventArgs
    {
        public T ItemSaved;
        public bool SavedNewItem;

        public DataServiceOnSaveOrUpdateArgs(T itemSaved, bool isNewItem) : base()
        {
            ItemSaved = itemSaved;
            SavedNewItem = isNewItem;
        }
    }

    public class DataServiceOnCollectionReSelectedArgs<T> : EventArgs
    {
        public int ItemsDifference;

        public DataServiceOnCollectionReSelectedArgs(int itemsDifference)
        {
            ItemsDifference = itemsDifference;
        }
    }
}