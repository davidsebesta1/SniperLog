using System.Globalization;

namespace SniperLog.Extensions;

/// <summary>
/// Static extension classes for parsing.
/// </summary>
public static class ParseExtensions
{
    /// <summary>
    /// Parses string to either <see langword="double"/> or <see langword="null"/> if input is <see cref="string.Empty"/> or <see langword="null"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>Double or null.</returns>
    public static double? ParseOrNullDouble(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return double.Parse(input, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses string to either <see langword="float"/> or <see langword="null"/> if input is <see cref="string.Empty"/> or <see langword="null"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>Float or null.</returns>
    public static float? ParseOrNullSingle(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return float.Parse(input, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses string to either <see langword="int"/> or <see langword="null"/> if input is <see cref="string.Empty"/> or <see langword="null"/>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>Integer or null.</returns>
    public static int? ParseOrNullInteger(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return int.Parse(input, CultureInfo.InvariantCulture);
    }
}

