
namespace SniperLog.Models.Interfaces
{
    public interface ICsvProcessable
    {
        public static virtual string CsvHeader { get; }

        public static virtual Task<ICsvProcessable> DeserializeFromCsvRow(string row)
        {
            throw new NotImplementedException();
        }

        public abstract string SerializeToCsvRow();
    }
}