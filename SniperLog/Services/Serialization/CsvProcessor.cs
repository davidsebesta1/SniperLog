using SniperLog.Models.Interfaces;
using System.Data;
using System.Reflection;

namespace SniperLog.Services.Serialization
{
    public class CsvProcessor
    {
        private readonly Dictionary<Type, string> _processorsByHeaders;

        public CsvProcessor()
        {
            var allCsvProcessable = typeof(ICsvProcessable).Assembly.GetTypes().Where(n => !n.IsAbstract && n.GetInterface("ICsvProcessable") != null);

            _processorsByHeaders = new Dictionary<Type, string>(allCsvProcessable.Count());
            foreach (Type type in allCsvProcessable)
            {
                var prop = type.GetProperty("CsvHeader", BindingFlags.Static | BindingFlags.Public);
                string header = ((string)prop.GetValue(null, null)).Trim();

                _processorsByHeaders.Add(type, header);
            }
        }

        public async Task LoadToDatabase<T>(StreamReader originalStream) where T : ICsvProcessable
        {
            using (TextReader reader = originalStream)
            {
                await reader.ReadLineAsync();
                string line = string.Empty;
                while (!string.IsNullOrEmpty((line = await reader.ReadLineAsync())))
                {
                    IDataAccessObject? obj = (IDataAccessObject?)T.DeserializeFromCsvRow(line);

                    if (obj != null)
                    {
                        await obj.SaveAsync();
                    }
                }
            }
        }

        public async Task GenerateCsv<T>(string folderPath, IEnumerable<T> enumerator, string fileName = "generated.csv") where T : ICsvProcessable
        {
            if (!enumerator.Any())
            {
                throw new ArgumentException("Unable to serialize empty enumerable");
            }
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException("Cannot export data to non-existing directory");
            }

            fileName = fileName.EndsWith(".csv") ? fileName : $"{fileName}.csv";

            using (StreamWriter stream = new StreamWriter(File.Create(Path.Combine(folderPath, fileName))))
            {
                await stream.WriteLineAsync(T.CsvHeader);
                var last = enumerator.Last();
                foreach (var item in enumerator)
                {
                    if (!item.Equals(last)) await stream.WriteLineAsync(item.SerializeToCsvRow());
                    else await stream.WriteAsync(item.SerializeToCsvRow());
                }
            }
        }
    }
}