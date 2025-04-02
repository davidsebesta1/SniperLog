using System.Text.RegularExpressions;

namespace DataAccessObjectAnalyzer.DataStructures
{
    /// <summary>
    /// Struct to define ForeignKey attribute syntax.
    /// </summary>
    public readonly struct ForeignKeyAttribute
    {
        /// <summary>
        /// Regex for "nameof(...)".
        /// </summary>
        public static Regex NameofRegex = new Regex("nameof\\(([^)]+)\\)");

        /// <summary>
        /// Regex for "typeof(...)".
        /// </summary>
        public static Regex TypeofRegex = new Regex("typeof\\(([^)]+)\\)");

        /// <summary>
        /// Referenced class.
        /// </summary>
        public readonly string ReferencedClass;

        /// <summary>
        /// Name of the property.
        /// </summary>
        public readonly string PropertyName;

        /// <summary>
        /// Ctor.
        /// </summary>
        public ForeignKeyAttribute(string referencedClass, string propertyName)
        {
            ReferencedClass = referencedClass;
            PropertyName = propertyName;
        }
    }
}
