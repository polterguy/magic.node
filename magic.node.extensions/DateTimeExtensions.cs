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
        /// <param name="assignToDefault">If true will assign the default timezone to dates without an explicit timezone part</param>
        /// <returns>DateTime possibly with attached timezone information</returns>
        public static DateTime EnsureTimezone(this DateTime value, bool assignToDefault = false)
        {
            if (assignToDefault && value.Kind == DateTimeKind.Unspecified && Converter.DefaultTimeZone != "none")
            {
                if (Converter.DefaultTimeZone == "utc")
                    return DateTime.SpecifyKind(value, DateTimeKind.Utc);
                if (Converter.DefaultTimeZone == "local")
                    return DateTime.SpecifyKind(value, DateTimeKind.Local);
            }
            return value;
        }

        /// <summary>
        /// Always converts DateTime to UTC
        /// </summary>
        /// <param name="value">Date to ensure</param>
        /// <returns>DateTime possibly with attached timezone information</returns>
        public static DateTime EnsureUTC(this DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);
            if (value.Kind == DateTimeKind.Local)
                return value.ToUniversalTime();
            return value;
        }
    }
}
