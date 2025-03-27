using Microsoft.Data.Sqlite;
using System.Data;

namespace SniperLog.Models
{
    public partial class SightReticle : ObservableObject, IDataAccessObject, IImageSaveable, IEquatable<SightReticle?>
    {
        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        private string _name;

        public string ImageSavePath
        {
            get
            {
                return Path.Combine("Data", "Reticles", Name, "backgroundimage.png");
            }
        }

        #endregion

        #region Ctor

        public SightReticle(int iD, string name)
        {
            ID = iD;
            Name = name;
        }

        public SightReticle(string name) : this(-1, name)
        {

        }

        #endregion

        #region DAO Methods

        public static IDataAccessObject LoadFromRow(DataRow row)
        {
            return new SightReticle(row);
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<SightReticle>>().Remove(this);
                DeleteImage();
            }
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
                else
                {
                    return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(InsertQuery, GetSqliteParams(true));
                }
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<SightReticle>>().AddOrUpdate(this);
            }
        }

        #endregion

        #region Object methods

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SightReticle);
        }

        public bool Equals(SightReticle? other)
        {
            return other is not null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(SightReticle? left, SightReticle? right)
        {
            return EqualityComparer<SightReticle>.Default.Equals(left, right);
        }

        public static bool operator !=(SightReticle? left, SightReticle? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
