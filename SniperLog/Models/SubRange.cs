using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using SniperLog.Services.Database;
using SniperLog.Services.Database.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Models
{
    public partial class SubRange : IDataAccessObject<SubRange>
    {
        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(ShootingRange), nameof(ShootingRange.ID))]
        public int ShootingRange_ID { get; set; }

        public ShootingRange? ReferencedShootingRange
        {
            get
            {
                return ShootingRange.GetById(ShootingRange_ID);
            }
        }

        public int RangeInMeters { get; set; }

        public double? Altitude { get; set; }

        public double? DirectionToNorth { get; set; }

        public double? VerticalFiringOffsetDegrees { get; set; }

        public string? NotesRelativePathFromAppData { get; set; }

        public static async Task<ObservableCollection<SubRange>> LoadAllAsync()
        {
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM SubRange");

            if (table == null)
            {
                throw new NullReferenceException("Table is null");
            }

            ObservableCollection<SubRange> collection = new ObservableCollection<SubRange>();
            foreach (DataRow row in table.Rows)
            {
                collection.Add(LoadFromRow(row));
            }

            return collection;
        }

        public static async Task<ObservableCollection<SubRange>> LoadAllAsync(ObservableCollection<SubRange> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Passed observable collection cannot be null");
            }

            collection.Clear();
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM SubRange");

            if (table == null)
            {
                throw new NullReferenceException("Table is null");
            }

            foreach (DataRow row in table.Rows)
            {
                collection.Add(LoadFromRow(row));
            }

            return collection;
        }

        public static async Task<SubRange?> LoadNewAsync(int id)
        {
            DataTable? table = await SqLiteDatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM SubRange WHERE SubRange.ID = @ID", new SqliteParameter("@ID", id));

            if (table == null)
            {
                return null;
            }

            if (table.Rows.Count > 1 || table.Rows.Count == 0)
            {
                throw new Exception($"Retrieved multiple instances from database of {nameof(ShootingRange)}, expected: 1 or 0");
            }
            return LoadFromRow(table.Rows[0]);
        }

        public static SubRange LoadFromRow(DataRow reader)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync()
        {
            return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync("DELETE FROM SubRange WHERE SubRange.ID = @ID", new SqliteParameter("@ID", ID)) == 1;
        }

        public async Task<int> SaveAsync()
        {
            throw new NotImplementedException();
        }

        public static SubRange? GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
