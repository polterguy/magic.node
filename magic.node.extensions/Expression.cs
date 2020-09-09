﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using magic.node.extensions.helpers;

namespace magic.node.extensions
{
    /// <summary>
    /// Expression class for creating lambda expressions, referencing nodes in your Node lambda objects.
    /// </summary>
    public class Expression
    {
        readonly List<Iterator> _iterators;

        /// <summary>
        /// Creates a new expression from its string representation.
        /// </summary>
        /// <param name="expression">String representation of expression to create.</param>
        public Expression(string expression)
        {
            _iterators = new List<Iterator>(Parse(expression));
        }

        /// <summary>
        /// Returns the string representation of your expression.
        /// </summary>
        public string Value
        {
            get
            {
                return string.Join("/", _iterators.Select(x =>
                {
                    // Checking if we need to quote iterator.
                    if (x.Value.Contains("/"))
                        return "\"" + x.Value + "\"";
                    return x.Value;
                }));
            }
        }

        /// <summary>
        /// Convenience method in case you want to access iterators individually.
        /// </summary>
        public IEnumerable<Iterator> Iterators { get { return _iterators; } }

        /// <summary>
        /// Evaluates your expression from the given identity node.
        /// </summary>
        /// <param name="identity">Identity node from which your expression is evaluated.</param>
        /// <returns>The result of the evaluation.</returns>
        public IEnumerable<Node> Evaluate(Node identity)
        {
            /*
             * Evaluating all iterators sequentially, returning the results to caller,
             * starting from the identity node.
             */
            IEnumerable<Node> result = new Node[] { identity };
            foreach (var idx in _iterators)
            {
                result = idx.Evaluate(identity, result);
                if (!result.Any())
                    return Array.Empty<Node>(); // Short circuiting to slightly optimize invocation.
            }
            return result;
        }

        #region [ -- Overrides -- ]

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Expression ex))
                return false;

            return Value.Equals(ex.Value);
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Parses your expression resulting in a chain of iterators.
         */
        IEnumerable<Iterator> Parse(string expression)
        {
            var builder = new StringBuilder();
            using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(expression))))
            {
                while (!reader.EndOfStream)
                {
                    var idx = (char)reader.Peek();
                    if (idx == '/')
                    {
                        yield return new Iterator(builder.ToString());
                        builder.Clear();
                        reader.Read(); // Discarding the '/' character at stream's head.
                    }
                    else if (idx == '"' && builder.Length == 0)
                    {
                        // Single quoted string, allows for having iterators containing "/" in their values.
                        builder.Append(ParserHelper.ReadQuotedString(reader));
                    }
                    else
                    {
                        builder.Append(idx);
                        reader.Read(); // Discarding whatever we stuffed into our builder just now.
                    }
                }
            }

            yield return new Iterator(builder.ToString());
        }

        #endregion
    }
}
