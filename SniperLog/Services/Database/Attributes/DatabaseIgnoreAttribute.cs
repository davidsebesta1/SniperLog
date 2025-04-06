
namespace SniperLog.Services.Database.Attributes;

/// <summary>
/// Custom DAO attribute for making generator analyzer ignore this field for database
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DatabaseIgnoreAttribute : Attribute
{

}
