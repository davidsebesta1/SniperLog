
using SniperLog.Extensions;

namespace SniperLog.Models.Interfaces
{
    /// <summary>
    /// Interface class allowing DAO object to save notes as .txt file based on implemeneted save path.
    /// </summary>
    public interface INoteSaveable
    {
        /// <summary>
        /// Relative path to the notes.txt starting at Data/...
        /// </summary>
        string NotesSavePath { get; }

        /// <summary>
        /// Returns a absolute path to the text file
        /// </summary>
        public virtual string NotesPathFull
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a text content of the file. If no file is found, an empty string is returned
        /// </summary>
        public virtual string NotesText
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Saves the notes based on save path
        /// </summary>
        /// <param name="notesText"></param>
        /// <returns></returns>
        public virtual Task SaveNotesAsync(string notesText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the notes file if it exists
        /// </summary>
        public virtual void DeleteNotes()
        {
            throw new NotImplementedException();
        }
    }
}
