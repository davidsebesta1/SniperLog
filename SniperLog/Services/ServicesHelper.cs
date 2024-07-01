
namespace SniperLog.Services
{
    public class ServicesHelper
    {
        public static IServiceProvider Services { get; private set; }

        public static void Init(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        public static T? GetService<T>()
        {
            if (Services is null) return default;
            return Services.GetService<T>();
        }

        public static object? GetService(Type type)
        {
            if (Services is null) return default;
            return Services.GetService(type);
        }
    }
}
