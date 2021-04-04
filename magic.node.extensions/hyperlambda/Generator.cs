/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Text;
using System.Collections.Generic;

namespace magic.node.extensions.hyperlambda
{
    /// <summary>
    /// Class to help convert a bunch of nodes into its Hyperlambda text representation.
    /// </summary>
    public static class Generator
    {
        /// <summary>
        /// Returns the Hyperlambda/string representation of the given list of nodes.
        /// Notice, will not serialize parent nodes, only nodes downwards in hierarchy.
        /// </summary>
        /// <param name="nodes">Root nodes to convert to Hyperlambda.</param>
        /// <returns>Hyperlambda representation of nodes.</returns>
        public static string GetHyper(IEnumerable<Node> nodes)
        {
            var result = new StringBuilder();
            GetHyper(result, nodes, 0);
            return result.ToString();
        }

        #region [ -- Private helper methods -- ]

        static void GetHyper(StringBuilder builder, IEnumerable<Node> nodes, int level)
        {
            foreach (var idx in nodes)
            {
                // Indenting correctly.
                int idxLevel = level;
                while (idxLevel-- > 0)
                    builder.Append("   ");

                // Serializing name into builder.
                AppendName(builder, idx, idx.Name);

                // Serializing value into builder, if any.
                AppendValue(builder, idx);

                // Adding Carriage Return, and serializing children, if any.
                builder.Append("\r\n");
                GetHyper(builder, idx.Children, level + 1);
            }
        }

        static void AppendValue(StringBuilder builder, Node idx)
        {
            if (idx.Value != null)
            {
                // Converting type to string.
                var value = Converter.ToString(idx.Value);
                builder.Append(":");

                // Checking if we need to provide an explicit type declaration.
                if (!string.IsNullOrEmpty(value.Item1) && value.Item1 != "string")
                    builder.Append(value.Item1 + ":");

                var stringValue = value.Item2;
                if (stringValue.Contains("\n"))
                {
                    stringValue = @"@""" + stringValue.Replace(@"""", @"""""") + @"""";
                }
                else if (stringValue.Contains(":") || 
                    stringValue.Contains("\"") ||
                    stringValue.Contains("'") ||
                    stringValue.StartsWith(" ") ||
                    stringValue.EndsWith(" "))
                {
                    stringValue = @"""" + stringValue.Replace(@"""", @"\""") + @"""";
                }

                // Appending actual value.
                builder.Append(stringValue);
            }
        }

        static void AppendName(StringBuilder builder, Node idx, string name)
        {
            if (name.Contains("\n"))
                name = "@\"" + name.Replace("\"", "\"\"") + "\"";
            else if (name.Contains("\"") ||
                name.Contains(":") ||
                name.StartsWith(" ") ||
                name.EndsWith(" "))
                name = "\"" + name.Replace("\"", "\\\"") + "\"";
            else if (idx.Value == null && name == "")
                name = @"""""";
            builder.Append(name);
        }

        #endregion
    }
}
