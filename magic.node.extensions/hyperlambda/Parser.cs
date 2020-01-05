/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Linq;
using magic.node.extensions.hyperlambda.internals;

namespace magic.node.extensions.hyperlambda
{
    /// <summary>
    /// Class allowing you to parse Hyperlambda from its textual representation,
    /// and create a lambda structure out of it.
    /// </summary>
    public sealed class Parser
    {
        readonly Node _root = new Node();

        /// <summary>
        /// Creates a new parser intended for parsing the specified Hyperlambda.
        /// </summary>
        /// <param name="hyperlambda">Hyperlambda you want to parse.</param>
        public Parser(string hyperlambda)
        {
            using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(hyperlambda))))
            {
                Parse(reader);
            }
        }

        /// <summary>
        /// Creates a new parser for Hyperlambda, parsing directly from the specified stream.
        /// </summary>
        /// <param name="stream">Stream to parse Hyperlambda from. Can be a forward
        /// only stream if necessary.</param>
        public Parser(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                Parse(reader);
            }
        }

        /// <summary>
        /// Returns the root lambda node parsed from your Hyperlambda.
        /// Notice, each node in your Hyperlambda at root level, will be a child
        /// node of the root node returned from this method.
        /// </summary>
        /// <returns>Root node containing all first level nodes from your Hyperlambda.</returns>
        public Node Lambda()
        {
            return _root;
        }

        /// <summary>
        /// Converts the specified string token to its equivalent object value,
        /// according to the type parameter given.
        ///
        /// The type parameter must be a Hyperlambda type, such as 'int', 'date',
        /// etc.
        /// </summary>
        /// <param name="value">String representation of your value.</param>
        /// <param name="type">Hyperlambda type to convert it to.</param>
        /// <returns>An object of type 'type' represented by the string 'value'.</returns>
        public static object ConvertStringToken(string value, string type)
        {
            return TypeConverter.ConvertFromString(value, type);
        }

        /// <summary>
        /// Converts the specified object token to its equivalent object value,
        /// according to the type parameter given.
        ///
        /// The type parameter must be a Hyperlambda type, such as 'int', 'date',
        /// etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="type">Hyperlambda type to convert it to.</param>
        /// <returns>An object of type 'type' represented by the string 'value'.</returns>
        public static object ConvertValue(object value, string type)
        {
            return TypeConverter.ConvertFromValue(value, type);
        }

        #region [ -- Private helper methods -- ]

        // TODO: Refactor and simplify. Too long and complex.
        void Parse(StreamReader reader)
        {
            var currentParent = _root;
            var tokenizer = new Tokenizer(reader);
            var en = tokenizer.GetTokens().GetEnumerator();

            Node idxNode = null;
            string previous = null;
            int level = 0;

            while (en.MoveNext())
            {
                var token = en.Current;
                switch (token)
                {
                    case ":":
                        if (idxNode == null)
                        {
                            idxNode = new Node();
                            currentParent.Add(idxNode);
                        }
                        else if (previous == ":")
                        {
                            idxNode.Value = ":";
                            break;
                        }

                        if (idxNode.Value == null)
                            idxNode.Value = "";
                        previous = token;
                        break;

                    case "\r\n":
                        idxNode = null; // Making sure we create a new node on next iteration.
                        previous = token;
                        break;

                    default:

                        // Checking if token is a scope declaration.
                        if (idxNode == null &&
                            token.StartsWith(" ", StringComparison.CurrentCulture) &&
                            !token.Any(x => x != ' '))
                        {
                            // We have a scope declaration.
                            int newLevel = token.Length / 3;
                            if (newLevel > level + 1)
                            {
                                // Syntax error in Hyperlambda, too many consecutive SP characters.
                                throw new ApplicationException("Too many spaces found in Hyperlambda content");
                            }
                            if (newLevel == level + 1)
                            {
                                // Children collection opens up.
                                currentParent = currentParent.Children.Last();
                                level = newLevel;
                            }
                            else
                            {
                                // Propagating upwards in ancestor hierarchy.
                                while (level > newLevel)
                                {
                                    currentParent = currentParent.Parent;
                                    --level;
                                }
                            }
                        }
                        else
                        {
                            if (previous == "\r\n")
                            {
                                // Special case for no spaces, and previous was CR.
                                currentParent = _root;
                                level = 0;
                            }

                            if (idxNode == null)
                            {
                                idxNode = new Node(token);
                                currentParent.Add(idxNode);
                            }
                            else if (idxNode.Value == null || "".Equals(idxNode.Value))
                            {
                                idxNode.Value = token;
                            }
                            else
                            {
                                idxNode.Value = TypeConverter.ConvertFromString(token, idxNode.Get<string>());
                            }
                        }
                        previous = token;
                        break;
                }
            }
        }

        #endregion
    }
}
