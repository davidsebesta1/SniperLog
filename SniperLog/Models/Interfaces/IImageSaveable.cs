using SniperLog.Extensions.WrapperClasses;

namespace SniperLog.Models.Interfaces
{
    public interface IImageSaveable
    {
        /// <summary>
        /// Relative path to the notes.txt starting at Data/...
        /// </summary>
        string ImageSavePath { get; }

        /// <summary>
        /// Returns a absolute path to the file
        /// </summary>
        public virtual string BackgroundImgPathFull
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a new <see cref="ImageSource"/> stream of the image
        /// </summary>
        public virtual ImageSource ImgStream
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the image to the predefined path
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public virtual async Task SaveImageAsync(DrawableImagePaths paths)
        { 
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the image if it exists
        /// </summary>
        public virtual void DeleteImage()
        {
            throw new NotImplementedException();
        }
    }
}