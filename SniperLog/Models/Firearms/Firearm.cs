using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using System.Data;

namespace SniperLog.Models
{
    public partial class Firearm : ObservableObject, IDataAccessObject, INoteSaveable, IEquatable<Firearm?>
    {
        #region Properties

        public string NotesSavePath
        {
            get
            {
                return Path.Combine("Data", "Firearms", Name, "notes.txt");
            }
        }

        [PrimaryKey]
        public int ID { get; set; }

        [ForeignKey(typeof(FirearmType), nameof(FirearmType.ID))]
        [ObservableProperty]
        private int _firearmType_ID;

        [ForeignKey(typeof(Manufacturer), nameof(Manufacturer.ID))]
        [ObservableProperty]
        private int? _manufacturer_ID;

        [ForeignKey(typeof(FirearmCaliber), nameof(FirearmCaliber.ID))]
        [ObservableProperty]
        private int _caliber_ID;

        [ForeignKey(typeof(FirearmSight), nameof(FirearmSight.ID))]
        [ObservableProperty]
        private int _firearmSight_ID;

        [ObservableProperty]
        public string _name;

        public string? Model { get; set; }

        public string? SerialNumber { get; set; }

        public double? TotalLengthMm { get; set; }

        public double? BarrelLengthInch { get; set; }

        public string? RateOfTwist { get; set; }

        public double? Weight { get; set; }

        public bool? HandednessForLeft { get; set; }

        #endregion

        #region Constructors

        public Firearm(int iD, int firearmType_ID, int? manufacturer_ID, int caliber_ID, int sight_ID, string name, string? model, string? serialNumber, double? totalLengthMm, double? barrelLengthInch, string? rateOfTwist, double? weight, bool? handednessForLeft)
        {
            ID = iD;
            FirearmType_ID = firearmType_ID;
            Manufacturer_ID = manufacturer_ID;
            Caliber_ID = caliber_ID;
            FirearmSight_ID = sight_ID;
            Name = name;
            Model = model;
            SerialNumber = serialNumber;
            TotalLengthMm = totalLengthMm;
            BarrelLengthInch = barrelLengthInch;
            RateOfTwist = rateOfTwist;
            Weight = weight;
            HandednessForLeft = handednessForLeft;
        }

        public Firearm(int firearmType_ID, int? manufacturer_ID, int caliber_ID, int sight_ID, string name, string? model, string? serialNumber, double? totalLengthMm, double? barrelLengthInch, string? rateOfTwist, double? weight, bool? handednessForLeft) : this(-1, firearmType_ID, manufacturer_ID, caliber_ID, sight_ID, name, model, serialNumber, totalLengthMm, barrelLengthInch, rateOfTwist, weight, handednessForLeft) { }

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
                ServicesHelper.GetService<DataCacherService<Firearm>>().AddOrUpdate(this);
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
                ServicesHelper.GetService<DataCacherService<Firearm>>().Remove(this);
                DeleteNotes();
            }
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return Name;
        }

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
            return left is Firearm f1 && right is Firearm f2 && f1.Equals(f2);
        }

        public static bool operator !=(Firearm? left, Firearm? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
