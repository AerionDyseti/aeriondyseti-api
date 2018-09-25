using System;

namespace AerionDyseti.Extensions
{
    /// <summary>
    /// Extension methods on the DateTime class.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime to the number of seconds since Jan 01 1970 (UTC).
        /// </summary>
        /// <param name="d">the Date to convert.</param>
        /// <returns>The number of seconds since the Unix Epoch.</returns>
        public static int ToUnixTimestamp(this DateTime d)
        {
            TimeSpan epoch = d - new DateTime(1970, 1, 1, 0, 0, 0);
            return ((int)epoch.TotalSeconds);
        }
    }
}
