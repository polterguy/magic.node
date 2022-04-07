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
        /// Ensures the date is converted into UTC unless date is UTC from before. Notice, if the DateTimeKind
        /// is "Unspecified" it assumes the date already is UTC and assigns UTC as its returned kind without
        /// changing the actual date and time value.
        /// </summary>
        /// <param name="value">Date to ensure</param>
        /// <param name="utcIsDefault">If true will specify UTC unless date already has time zone specified</param>
        /// <returns>UTC date equivalent</returns>
        public static DateTime EnsureTimezone(this DateTime value, bool utcIsDefault = false)
        {
            if (value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(value, utcIsDefault || Converter.AssumeUtc ? DateTimeKind.Utc : DateTimeKind.Local);
            return value;
        }
    }
}
