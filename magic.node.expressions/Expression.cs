/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace magic.node.expressions
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
        /// <param name="expression"></param>
        public Expression(string expression)
        {
            Value = expression;
            _iterators = new List<Iterator>(Parse(expression));
        }

        /// <summary>
        /// Returns the string representation of your expression.
        /// </summary>
        public string Value { get; private set; }

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
            // Evaluating all iterators.
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

        /// <summary>
        /// Returns a string representation of your Expression.
        /// </summary>
        /// <returns>A string representation of your expression.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Returns the hash code for your instance.
        /// </summary>
        /// <returns>Hash code, useful for for instance creating keys for dictionaries, etc.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Comparison method, comparing the current instance to some other instance.
        /// </summary>
        /// <param name="obj">Right hand side to compare instance with.</param>
        /// <returns>True if instances are logically similar.</returns>
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

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var idx = (char)reader.Peek();
                        if (idx == -1)
                            break;

                        if (idx == '/')
                        {
                            yield return new Iterator(builder.ToString());
                            builder.Clear();
                        }
                        else
                        {
                            builder.Append(idx);
                        }
                        reader.Read(); // Simply discarding currently handled character.
                    }
                }
            }

            if (builder.Length == 0)
                throw new ApplicationException("Syntax error at end of expression, unexpected trailing '/' found at end of expression.");

            yield return new Iterator(builder.ToString());
        }

        #endregion
    }
}
