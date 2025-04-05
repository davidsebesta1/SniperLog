
namespace SniperLog.Services.Database.Attributes;

/// <summary>
/// Custom attribute for primary keys.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class PrimaryKeyAttribute : Attribute
{

}
