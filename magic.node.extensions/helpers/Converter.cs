/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using magic.node.extensions.hyperlambda;

namespace magic.node.extensions.helpers
{
    /// <summary>
    /// Helper class for converting from string representations to Hyperlambda declaration objects, and vice versa.
    /// </summary>
    public static class Converter
    {
        #region [ -- Converters -- ]

        // String => Func Dictionary, containing actual object to object converters for built-in types.
        readonly static Dictionary<string, Func<object, object>> _toObjectFunctors =
            new Dictionary<string, Func<object, object>>()
        {
            {"short", (value) => Convert.ToInt16(value, CultureInfo.InvariantCulture)},
            {"ushort", (value) => Convert.ToUInt16(value, CultureInfo.InvariantCulture)},
            {"int", (value) => Convert.ToInt32(value, CultureInfo.InvariantCulture)},
            {"uint", (value) => Convert.ToUInt32(value, CultureInfo.InvariantCulture)},
            {"long", (value) => Convert.ToInt64(value, CultureInfo.InvariantCulture)},
            {"ulong", (value) => Convert.ToUInt64(value, CultureInfo.InvariantCulture)},
            {"decimal", (value) => Convert.ToDecimal(value, CultureInfo.InvariantCulture)},
            {"double", (value) => Convert.ToDouble(value, CultureInfo.InvariantCulture)},
            {"single", (value) => Convert.ToSingle(value, CultureInfo.InvariantCulture)},
            {"float", (value) => Convert.ToSingle(value, CultureInfo.InvariantCulture)},
            {"char", (value) => Convert.ToChar(value, CultureInfo.InvariantCulture)},
            {"byte", (value) => Convert.ToByte(value, CultureInfo.InvariantCulture)},
            {"string", (value) => {
                return 
                    (value as string) ??
                    (value == null ?
                        "" :
                        _toStringFunctors[value.GetType().FullName](value).Item2);
            }},
            {"bool", (value) => {
                if (value is bool)
                    return value;
                return value.Equals("true");
            }},
            {"date", (value) => {
                if (value is DateTime)
                    return value;
                return DateTime.ParseExact(
                    value.ToString(),
                    "yyyy-MM-ddTHH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal)
                    .ToUniversalTime();
            }},
            {"time", (value) => {
                if (value is TimeSpan)
                    return value;
                return new TimeSpan(Convert.ToInt64(value, CultureInfo.InvariantCulture));
            }},
            {"guid", (value) => {
                if (value is Guid)
                    return value;
                return new Guid(value.ToString());
            }},
            {"x", (value) => {
                if (value is Expression)
                    return value;
                return new Expression(value.ToString());
            }},
            {"node", (value) => {
                if (value is Node)
                    return value;
                return new Parser(value.ToString()).Lambda();
            }},
        };

        // String => Func Dictionary, containing actual object to string converters for built-in types.
        readonly static Dictionary<string, Func<object, (string, string)>> _toStringFunctors = new Dictionary<string, Func<object, (string, string)>>()
        {
            { "System.Boolean", (value) => {
                return ("bool", ((bool)value).ToString().ToLower());
            }},
            { "System.String", (value) => {
                return ("string", (string)value);
            }},
            { "System.Int16", (value) => {
                return ("short", ((short)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.UInt16", (value) => {
                return ("ushort", ((ushort)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Int32", (value) => {
                return ("int", ((int)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.UInt32", (value) => {
                return ("uint", ((uint)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Int64", (value) => {
                return ("long", ((long)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.UInt64", (value) => {
                return ("ulong", ((ulong)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Decimal", (value) => {
                return ("decimal", ((decimal)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Double", (value) => {
                return ("double", ((double)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Single", (value) => {
                return ("float", ((float)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.DateTime", (value) => {
                return ("date", ((DateTime)value)
                    .ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }},
            { "System.TimeSpan", (value) => {
                return ("time", ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Guid", (value) => {
                return ("guid", ((Guid)value).ToString());
            }},
            { "System.Char", (value) => {
                return ("char", ((char)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "System.Byte", (value) => {
                return ("byte", ((byte)value).ToString(CultureInfo.InvariantCulture));
            }},
            { "magic.node.extensions.Expression", (value) => {
                return ("x", ((Expression)value).Value);
            }},
            { "magic.node.Node", (value) => {
                return ("node", Generator.GetHyper(((Node)value).Children));
            }},
        };

        #endregion


        /// <summary>
        /// Converts the given string value to the type declaration specified as the type parameter.
        /// </summary>
        /// <param name="value">Object value.</param>
        /// <param name="type">Type to convert object into</param>
        /// <returns>Converted object.</returns>
        public static object ToObject(object value, string type)
        {
            if (!_toObjectFunctors.ContainsKey(type))
                throw new ArgumentException($"Unknown type declaration '{type}'");
            return _toObjectFunctors[type](value);
        }

        /// <summary>
        /// Converts value of object into a string, intended to be serialized into Hyperlambda format,
        /// and returns its Hyperlambda type declaration, and the string representation of the object to caller.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Hyperlambda type declaration, and string representation of object.</returns>
        public static (string, string) ToString(object value)
        {
            if (value == null)
                return (null, null);
            var typeName = value.GetType().FullName;
            if (!_toStringFunctors.ContainsKey(typeName))
                throw new ArgumentException($"I don't know how to convert from '{typeName}' to a string representation.");
            return _toStringFunctors[typeName](value);
        }

        /// <summary>
        /// Adds a custom type to the converter, allowing you to support your own custom types
        /// in Hyperlambda.
        /// </summary>
        /// <param name="clrType">The CLR type you wish to support</param>
        /// <param name="hyperlambdaTypename">Its Hyperlambda type name</param>
        /// <param name="toStringFunctor">Functor expected to create a string representation of an instance of your type.</param>
        /// <param name="toObjectFunctor">Functor expected to create an object of your type, given its Hyperlambda string representation.</param>
        public static void AddConverter(
            Type clrType,
            string hyperlambdaTypename,
            Func<object, (string, string)> toStringFunctor,
            Func<object, object> toObjectFunctor)
        {
            _toStringFunctors[clrType.FullName] = toStringFunctor;
            _toObjectFunctors[hyperlambdaTypename] = toObjectFunctor;
        }

        /// <summary>
        /// Returns all sypported types for the Hyperlambda parser.
        /// </summary>
        /// <returns>List of Hyperlambda type names, supported by the parser, including custom types.</returns>
        public static IEnumerable<string> ListTypes()
        {
            return _toObjectFunctors.Keys;
        }
    }
}
