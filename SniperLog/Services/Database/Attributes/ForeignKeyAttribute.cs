namespace SniperLog.Services.Database.Attributes;

/// <summary>
/// Custom DAO Attribute for analyzer to generate Reference for a instance of a object
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ForeignKeyAttribute : Attribute
{
    /// <summary>
    /// References class.
    /// </summary>
    public Type ReferencedClass { get; }

    /// <summary>
    /// Property name of ID of the references class.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Ctor.
    /// </summary>
    public ForeignKeyAttribute(Type refClass, string propertyName)
    {
        ReferencedClass = refClass;
        PropertyName = propertyName;
    }
}