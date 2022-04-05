/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;

namespace magic.node.extensions
{
    /// <summary>
    /// Extension class extending the Node class with convenience methods.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Ensures the date is converted into UTC unless date is UTC from before, and or unspecified kind,
        /// at which point it assumes the date already is UTC.
        /// </summary>
        /// <param name="value">Date to ensure</param>
        /// <returns>UTC kind of date</returns>
        public static DateTime EnsureUtc(this DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Utc:
                    return value;

                case DateTimeKind.Unspecified:
                    return DateTime.SpecifyKind(value, DateTimeKind.Utc);

                default:
                    return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, DateTimeKind.Utc);
            }
        }
    }
}
