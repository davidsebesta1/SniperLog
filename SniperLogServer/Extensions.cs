using SniperLogNetworkLibrary.CommonLib;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace SniperLogServer;

/// <summary>
/// Source: https://github.com/dotnet/runtime/issues/13051
/// </summary>
public static class Extensions
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    static extern uint GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);
    static readonly int MAX_PATH = 255;

    public static string GetExecutablePath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StringBuilder sb = StringBuilderPool.Get();
            GetModuleFileName(IntPtr.Zero, sb, MAX_PATH);
            return StringBuilderPool.ReturnToString(sb);
        }
        else
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }
    }
}

/// <summary>
/// Extensions for the <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Trims the end of the inner array.
    /// Source: https://stackoverflow.com/questions/24769701/trim-whitespace-from-the-end-of-a-stringbuilder-without-calling-tostring-trim
    /// </summary>
    /// <param name="sb">The string builder.</param>
    public static StringBuilder TrimEnd(this StringBuilder sb)
    {
        if (sb == null || sb.Length == 0)
            return sb;

        int i = sb.Length - 1;

        for (; i >= 0; i--)
            if (!char.IsWhiteSpace(sb[i]))
                break;

        if (i < sb.Length - 1)
            sb.Length = i + 1;

        return sb;
    }

    /// <summary>
    /// Appends a obj's <see cref="object.ToString()"/> and new line.
    /// </summary>
    /// <param name="sb">This stringbuilder.</param>
    /// <param name="obj">Target object to append.</param>
    /// <returns>This stringbuilder.</returns>
    public static StringBuilder AppendLine(this StringBuilder sb, object obj)
    {
        sb.AppendLine(obj.ToString());
        return sb;
    }
}

/// <summary>
/// Extensions for the <see cref="DateTime"/>.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Gets whether this date is in the same week as the parameter one.
    /// Source: https://stackoverflow.com/questions/25795254/check-if-a-datetime-is-in-same-week-as-other-datetime
    /// </summary>
    /// <param name="dt1">This date.</param>
    /// <param name="dt2">Target date.</param>
    /// <returns>Whether this date is in the same week as the other one.</returns>
    public static bool IsInSameWeek(this DateTime dt1, DateTime dt2)
    {
        Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
        DateTime d1 = dt1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(dt1));
        DateTime d2 = dt2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(dt2));

        return d1 == d2;
    }
}
