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
    /// Gets a <see cref="StringBuilder"/>.
    /// </summary>
    /// <returns><see cref="StringBuilder"/> instance.</returns>
    public static StringBuilder Get()
    {
        if (!_stored.TryTake(out StringBuilder sb))
            return new StringBuilder(4096);

        return sb;
    }

    /// <summary>
    /// Returns <see cref="StringBuilder"/> to the pool.
    /// </summary>
    /// <param name="sb"><see cref="StringBuilder"/> to be returned.</param>
    public static void Return(StringBuilder sb)
    {
        sb.Clear();
        _stored.Add(sb);
    }

    /// <summary>
    /// Returns <see cref="StringBuilder"/> to the pool and returns result <see langword="string"/>.
    /// </summary>
    /// <param name="sb">Stringbuilder to be returned.</param>
    /// <returns>Result <see cref="StringBuilder.ToString()"/> of the <see cref="StringBuilder"/>.</returns>
    public static string ReturnToString(StringBuilder sb)
    {
        string result = sb.ToString();
        Return(sb);
        return result;
    }
}
