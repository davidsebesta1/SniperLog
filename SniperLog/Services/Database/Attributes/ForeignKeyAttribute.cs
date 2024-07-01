namespace SniperLog.Services.Database.Attributes
{
    /// <summary>
    /// Custom DAO Attribute for analyzer to generate Reference for a instance of a object
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ForeignKeyAttribute : Attribute
    {
        public Type ReferencedClass { get; }
        public string PropertyName { get; }

        public ForeignKeyAttribute(Type refClass, string propertyName)
        {
            this.ReferencedClass = refClass;
            this.PropertyName = propertyName;
        }
    }
}