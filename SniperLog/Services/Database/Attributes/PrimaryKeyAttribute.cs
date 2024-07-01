
namespace SniperLog.Services.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {

    }
}
