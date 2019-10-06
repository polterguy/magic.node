/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Globalization;
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
                    return DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);

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
                    // TODO: Implement support for dynamically figuring out how to create types from strings.
                    throw new ApplicationException($"Unknown type declaration '{type}'");
            }
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

                    // TODO: Implement support for control characters (\t, etc)
                    if (value.Contains("\n"))
                        value = "@\"" + value.Replace("\"", "\"\"") + "\"";
                    else if (value.Contains("\"") || value.Contains(":"))
                        value = "\"" + value.Replace("\"", "\\\"") + "\"";
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
                    value = node.Get<DateTime>().ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
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
                    type = "bool";
                    value = node.Get<byte>().ToString(CultureInfo.InvariantCulture);
                    break;

                case "magic.node.Expression":
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
