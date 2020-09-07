/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using magic.node.expressions;

namespace magic.node.extensions.hyperlambda.internals
{
    /*
     * Helper class for converting from string representations to Hyperlambda declaration objects, and vice versa.
     */
    internal static class TypeConverter
    {
        /*
         * Converts the given string value to the type declaration specified as the type parameter.
         */
        public static object ConvertFromString(string value, string type)
        {
            switch (type)
            {
                case "string":
                    return value;

                case "short":
                    return Convert.ToInt16(value, CultureInfo.InvariantCulture);

                case "ushort":
                    return Convert.ToUInt16(value, CultureInfo.InvariantCulture);

                case "int":
                    return Convert.ToInt32(value, CultureInfo.InvariantCulture);

                case "uint":
                    return Convert.ToUInt32(value, CultureInfo.InvariantCulture);

                case "long":
                    return Convert.ToInt64(value, CultureInfo.InvariantCulture);

                case "ulong":
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

                case "decimal":
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);

                case "double":
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture);

                case "single":
                    return Convert.ToSingle(value, CultureInfo.InvariantCulture);

                case "bool":
                    return value.Equals("true");

                case "date":
                    return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                case "time":
                    return new TimeSpan(long.Parse(value));

                case "guid":
                    return new Guid(value);

                case "char":
                    return Convert.ToChar(value, CultureInfo.InvariantCulture);

                case "byte":
                    return Convert.ToByte(value, CultureInfo.InvariantCulture);

                case "x":
                    return new Expression(value);

                case "node":
                    return new Parser(value).Lambda();

                default:

                    throw new ArgumentException($"Unknown type declaration '{type}'");
            }
        }

        static Dictionary<string, Func<object, object>> _convertFromValue = new Dictionary<string, Func<object, object>>()
        {
            {"string", (value) => (value as string) ?? value.ToString()},
            {"short", (value) => {
                return Convert.ToInt16(value, CultureInfo.InvariantCulture);
            }},
            {"ushort", (value) => {
                return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
            }},
            {"int", (value) => {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }},
            {"uint", (value) => {
                return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
            }},
            {"long", (value) => {
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }},
            {"ulong", (value) => {
                return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }},
            {"decimal", (value) => {
                return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }},
            {"double", (value) => {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }},
            {"single", (value) => {
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }},
            {"float", (value) => {
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
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
                    DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }},
            {"time", (value) => {
                if (value is TimeSpan)
                    return value;
                return new TimeSpan((long)value);
            }},
            {"guid", (value) => {
                if (value is Guid)
                    return value;
                return new Guid(value.ToString());
            }},
            {"char", (value) => {
                return Convert.ToChar(value, CultureInfo.InvariantCulture);
            }},
            {"byte", (value) => {
                return Convert.ToByte(value, CultureInfo.InvariantCulture);
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

        /*
         * Converts the given string value to the type declaration specified as the type parameter.
         */
        public static object ConvertFromValue(object value, string type)
        {
            if (!_convertFromValue.ContainsKey(type))
                throw new ArgumentException($"Unknown type declaration '{type}'");
            return _convertFromValue[type](value);
        }

        /*
         * Converts the given Node's value to its string representation, in
         * addition to returning its type information.
         */
        public static string ConvertToString(Node node, out string type)
        {
            string value;
            switch (node.Value.GetType().FullName)
            {
                case "System.String":
                    type = "string";
                    value = node.Get<string>();

                    if (value.Contains("\n"))
                        value = "@\"" + value.Replace("\"", "\"\"") + "\"";
                    else if (value.Contains("\"") || value.Contains(":"))
                        value = "\"" + value.Replace("\"", "\\\"") + "\"";
                    break;

                case "System.Int16":
                    type = "short";
                    value = node.Get<short>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.UInt16":
                    type = "ushort";
                    value = node.Get<ushort>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Int32":
                    type = "int";
                    value = node.Get<int>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.UInt32":
                    type = "uint";
                    value = node.Get<uint>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Int64":
                    type = "long";
                    value = node.Get<long>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.UInt64":
                    type = "ulong";
                    value = node.Get<ulong>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Decimal":
                    type = "decimal";
                    value = node.Get<decimal>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Double":
                    type = "double";
                    value = node.Get<double>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Single":
                    type = "float";
                    value = node.Get<float>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Boolean":
                    type = "bool";
                    value = node.Get<bool>().ToString(CultureInfo.InvariantCulture).ToLower();
                    break;

                case "System.DateTime":
                    type = "date";
                    value = @"""" +
                        node.Get<DateTime>()
                            .ToUniversalTime()
                            .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") +
                        @"""";
                    break;

                case "System.TimeSpan":
                    type = "time";
                    value = node.Get<TimeSpan>().Ticks.ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Guid":
                    type = "bool";
                    value = node.Get<Guid>().ToString();
                    break;

                case "System.Char":
                    type = "char";
                    value = node.Get<char>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "System.Byte":
                    type = "byte";
                    value = node.Get<byte>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "magic.node.expressions.Expression":
                    type = "x";
                    value = node.Get<Expression>().Value;
                    break;

                case "magic.node.Node":
                    type = "node";
                    value = "@\"" + Generator.GetHyper(node.Get<Node>().Children).Replace("\"", "\"\"") + "\"";
                    break;

                default:
                    value = node.Get<string>();
                    type = node.Value.GetType().FullName;
                    if (value.Contains("\n"))
                        value = "@\"" + value.Replace("\"", "\"\"") + "\"";
                    else if (value.Contains("\"") || value.Contains(":"))
                        value = "\"" + value.Replace("\"", "\\\"") + "\"";
                    break;
            }
            return value;
        }
    }
}
