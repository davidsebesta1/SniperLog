
namespace SniperLogNetworkLibrary.CommonLib
{
    public static class CardinalDirectionConverter
    {
        private static readonly CardinalDirection[] _cardinalDirections =
            [
            new CardinalDirection("North", 335,25),
            new CardinalDirection("North-East", 25, 65),
            new CardinalDirection("East", 65, 110),
            new CardinalDirection("South-East", 110, 155),
            new CardinalDirection("South", 155, 200),
            new CardinalDirection("South-West", 200, 245),
            new CardinalDirection("South-West", 245, 290),
            new CardinalDirection("South-West", 290, 335)
            ];

        public static string GetNameByDegree(int degree)
        {
            CardinalDirection dir = _cardinalDirections.FirstOrDefault(n => n.Contains(degree));

            if (dir.Equals(default(CardinalDirection)))
            {
                return "Unknown Direction";
            }

            return dir.Name;
        }
    }

    /// <summary>
    /// Struct representing cardinal direction by its name and degree ranges
    /// </summary>
    public readonly struct CardinalDirection
    {
        /// <summary>
        /// Name of the direction
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Starting degree of the direction, exclusive
        /// </summary>
        public readonly int StartingDegree;

        /// <summary>
        /// Ending degree of the direction, inclusive
        /// </summary>
        public readonly int EndingDegree;

        public CardinalDirection(string name, int startingDegree, int endingDegree)
        {
            Name = name;
            StartingDegree = startingDegree;
            EndingDegree = endingDegree;
        }

        /// <summary>
        /// Returns information whenever the input degree is within direction range
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <returns>True if degree is in valid range. False if not or input is less than 0 or more than 360</returns>
        public bool Contains(int degree)
        {
            if (degree < 0 || degree > 360)
            {
                return false;
            }

            if (StartingDegree > EndingDegree)
            {
                int shift = (360 - StartingDegree);

                degree = (degree + shift) % 360;
            }

            return degree >= StartingDegree && degree <= EndingDegree;
        }
    }
}
