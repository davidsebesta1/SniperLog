
namespace SniperLog.Models.Interfaces
{
    public interface ICsvProcessable
    {
        public static abstract string CsvHeader { get; }

        public static abstract ICsvProcessable DeserializeFromCsvRow(string row);

        public abstract string SerializeToCsvRow();
    }
}