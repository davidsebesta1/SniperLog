using System.Collections.Concurrent;
using System.Text;
namespace SniperLog.Extensions.Pools
{
    public class StringBuilderPool
    {
        private readonly ConcurrentQueue<StringBuilder> _pool = new ConcurrentQueue<StringBuilder>();

        private static StringBuilderPool _instance;
        public static StringBuilderPool Instance
        {
            get
            {
                return _instance ??= new StringBuilderPool();
            }
        }

        public StringBuilder Rent()
        {
            if (_pool.Count == 0)
            {
                return new StringBuilder(512);
            }

            if (!_pool.TryDequeue(out StringBuilder builder))
            {
                return new StringBuilder(512);
            }

            return builder;
        }

        public void Return(StringBuilder builder)
        {
            builder.Clear();
            _pool.Enqueue(builder);
        }

        public string ReturnToString(StringBuilder builder)
        {
            string res = builder.ToString();
            builder.Clear();
            _pool.Enqueue(builder);
            return res;
        }
    }
}