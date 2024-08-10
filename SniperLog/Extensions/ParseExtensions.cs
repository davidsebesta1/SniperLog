using System.Globalization;

namespace SniperLog.Extensions
{
    public static class ParseExtensions
    {
        public static double? ParseOrNullDouble(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return double.Parse(input, CultureInfo.InvariantCulture);
        }

        public static float? ParseOrNullSingle(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return float.Parse(input, CultureInfo.InvariantCulture);
        }

        public static int? ParseOrNullInteger(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return int.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}
