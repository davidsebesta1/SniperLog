using Microsoft.Data.Sqlite;
using SniperLog.Extensions;
using System.Data;

namespace SniperLog.Models
{
    public partial class SightReticle : ObservableObject, IDataAccessObject, IEquatable<SightReticle?>
    {
        #region Constants

        public const string BackgroundImageFileName = "backgroundimage.png";

        #endregion

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundImgFullPath))]
        private string _backgroundImgPath;

        [DatabaseIgnore]
        public string BackgroundImgFullPath
        {
            get
            {
                string path = AppDataFileHelper.GetPathFromAppData(BackgroundImgPath);
                if (!Path.Exists(path))
                {
                    return "defaultbackground.png";
                }

                return path;
            }
        }

        #endregion

        #region Ctor

        public SightReticle(int iD, string name, string backgroundImgPath)
        {
            ID = iD;
            Name = name;
            BackgroundImgPath = backgroundImgPath;
        }

        public SightReticle(string name, string backgroundImgPath) : this(-1, name, backgroundImgPath)
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
                string dataPath = AppDataFileHelper.GetPathFromAppData(Path.Combine("Data", "ShootingRange", Name));
                if (Directory.Exists(dataPath))
                {
                    Directory.Delete(dataPath, true);
                }

                return await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(DeleteQuery, new SqliteParameter("@ID", ID)) == 1;
            }
            finally
            {
                ServicesHelper.GetService<DataCacherService<SightReticle>>().Remove(this);
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

        #region Model Specific Methods

        /// <summary>
        /// Saves the image to the predefined path
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task SaveImageAsync(FileStream stream)
        {
            string localPath = Path.Combine("Data", "FirearmSights", Name);
            string localFilePath = Path.Combine(localPath, BackgroundImageFileName);

            string fullDirPath = AppDataFileHelper.GetPathFromAppData(localPath);
            Directory.CreateDirectory(fullDirPath);

            string fullFilepath = Path.Combine(fullDirPath, BackgroundImageFileName);

            using (FileStream localFileStream = File.OpenWrite(fullFilepath))
            {
                await stream.CopyToAsync(localFileStream);
            }

            BackgroundImgPath = localFilePath;
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
