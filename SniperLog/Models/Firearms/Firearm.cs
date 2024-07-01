using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using SniperLog.Services.Database.Attributes;
using System.Data;

namespace SniperLog.Models
{
    public partial class Firearm : ObservableObject, IDataAccessObject, IEquatable<Firearm?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(FirearmType), nameof(FirearmType.ID))]
        [ObservableProperty]
        private int _firearmType_ID;

        [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
        public int? Manufacturer_ID { get; set; }

        [ForeignKey(typeof(FirearmCaliber), nameof(FirearmCaliber.ID))]
        [ObservableProperty]
        private int _caliber_ID;

        [ObservableProperty]
        public string _name;

        public string? Model { get; set; }

        public string? SerialNumber { get; set; }

        public double? TotalLengthMm { get; set; }

        public double? BarrelLengthInch { get; set; }

        public string? RateOfTwist { get; set; }

        public double? Weight { get; set; }

        public bool? HandednessForLeft { get; set; }

        public string? NotesRelativePathFromAppData { get; set; }

        #endregion

        #region Constructors

        public Firearm(int iD, int firearmType_ID, int? manufacturer_ID, int caliber_ID, string name, string? model, string? serialNumber, double? totalLengthMm, double? barrelLengthInch, string? rateOfTwist, double? weight, bool? handednessForLeft, string? notesRelativePathFromAppData)
        {
            ID = iD;
            FirearmType_ID = firearmType_ID;
            Manufacturer_ID = manufacturer_ID;
            Caliber_ID = caliber_ID;
            Name = name;
            Model = model;
            SerialNumber = serialNumber;
            TotalLengthMm = totalLengthMm;
            BarrelLengthInch = barrelLengthInch;
            RateOfTwist = rateOfTwist;
            Weight = weight;
            HandednessForLeft = handednessForLeft;
            NotesRelativePathFromAppData = notesRelativePathFromAppData;
        }

        public Firearm(int firearmType_ID, int? manufacturer_ID, int caliber_ID, string name, string? model, string? serialNumber, double? totalLengthMm, double? barrelLengthInch, string? rateOfTwist, double? weight, bool? handednessForLeft, string? notesRelativePathFromAppData) : this(-1, firearmType_ID, manufacturer_ID, caliber_ID, name, model, serialNumber, totalLengthMm, barrelLengthInch, rateOfTwist, weight, handednessForLeft, notesRelativePathFromAppData) { }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new Firearm(row);
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                if (ID == -1)
                {
                    ID = await SqLiteDatabaseConnection.Instance.ExecuteScalarIntAsync(InsertQueryNoId, GetSqliteParams(false));
                    return ID;
                }
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery,
                       GetSqliteParams(true));
            }
            finally
            {
                await ServicesHelper.GetService<DataCacherService<Firearm>>().AddOrUpdateAsync(this);
            }
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                await ServicesHelper.GetService<DataCacherService<Firearm>>().RemoveAsync(this);
            }
        }

        #endregion

        #region Other

        public async Task SaveNotes(string newNotes)
        {
            string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "Firearms", $"Firearm{ID}", $"notes.txt"));

            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
            NotesRelativePathFromAppData = dataPath;
            await File.WriteAllTextAsync(dataPath, newNotes);
            await SaveAsync();
        }


        #endregion

        #region Equals

        public override bool Equals(object? obj)
        {
            return Equals(obj as Firearm);
        }

        public bool Equals(Firearm? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Firearm? left, Firearm? right)
        {
            return EqualityComparer<Firearm>.Default.Equals(left, right);
        }

        public static bool operator !=(Firearm? left, Firearm? right)
        {
            return !(left == right);
        }



        #endregion
    }
}
