using System.Collections.Concurrent;
using System.Text;

namespace SniperLogNetworkLibrary.CommonLib;

/// <summary>
/// Pool of <see cref="StringBuilder"/>s.
/// </summary>
public static class StringBuilderPool
{
    private static readonly ConcurrentBag<StringBuilder> _stored = new ConcurrentBag<StringBuilder>();

    /// <summary>
    /// Gets a stringbuilder.
    /// </summary>
    /// <returns>String Builder instance.</returns>
    public static StringBuilder Get()
    {
        if (!_stored.TryTake(out StringBuilder sb))
            return new StringBuilder(4096);

        return sb;
    }

    /// <summary>
    /// Returns stringbuilder to the pool.
    /// </summary>
    /// <param name="sb">Stringbuilder to be returned.</param>
    public static void Return(StringBuilder sb)
    {
        sb.Clear();
        _stored.Add(sb);
    }

    /// <summary>
    /// Returns stringbuilder to the pool and returns result string.
    /// </summary>
    /// <param name="sb">Stringbuilder to be returned.</param>
    /// <returns>Result tostring of the stringbuilder.</returns>
    public static string ReturnToString(StringBuilder sb)
    {
        string result = sb.ToString();
        Return(sb);
        return result;
    }
}
