using System;

namespace Wulka.Core
{
    /// <summary>
    /// Exposes properties to retrieve the current Date and Time in UTC.
    /// </summary>
    public class Time
    {
        /// <summary>
        /// An offset to the UTC DateTime giving the current Date and Time
        /// </summary>
        public static TimeSpan UTCOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        /// <value>The UTC time.</value>
        public static DateTime UTCTime
        {
            get
            {
                return DateTime.Now - UTCOffset;
            }
        }

        /// <summary>
        /// Gets the current UTC Date.
        /// </summary>
        /// <value>The UTC today.</value>
        public static DateTime UTCToday
        {
            get
            {
                return UTCTime.Date;
            }
        }
    }
}
