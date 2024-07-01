using System.Text.RegularExpressions;

namespace DataAccessObjectAnalyzer.DataStructures
{
    public readonly struct ForeignKeyAttribute
    {
        public static Regex NameofRegex = new Regex("nameof\\(([^)]+)\\)");
        public static Regex TypeofRegex = new Regex("typeof\\(([^)]+)\\)");

        public readonly string ReferencedClass;
        public readonly string PropertyName;

        public ForeignKeyAttribute(string referencedClass, string propertyName)
        {
            ReferencedClass = referencedClass;
            PropertyName = propertyName;
        }
    }
}
