using System.Reflection;

namespace SniperLog.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsStatic(this PropertyInfo propertyInfo, bool notPublic = false)
        {
            return propertyInfo.GetAccessors(notPublic).Any(x => x.IsStatic);
        }
    }
}