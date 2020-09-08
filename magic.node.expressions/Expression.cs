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
                        builder.Append(ReadQuotedString(reader));
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

        /*
         * Reads a single line string literal, basically a string surrounded by only "".
         */
        static string ReadQuotedString(StreamReader reader)
        {
            var endCharacter = (char)reader.Read();
            var builder = new StringBuilder();
            for (var c = reader.Read(); c != -1; c = reader.Read())
            {
                if (c == endCharacter)
                    return builder.ToString();

                switch (c)
                {
                    case '\\':
                        builder.Append(GetEscapeCharacter(reader, endCharacter));
                        break;

                    case '\n':
                    case '\r':
                        throw new ArgumentException("Syntax error, string literal unexpected CR/LF");

                    default:
                        builder.Append((char)c);
                        break;
                }
            }
            throw new ArgumentException("Syntax error, string literal not closed before end of input");
        }

        /*
         * Reads a single character from a single line string literal, escaped
         * with the '\' character.
         */
        static string GetEscapeCharacter(StreamReader reader, char endCharacter)
        {
            var ch = reader.Read();
            if (ch == endCharacter)
                return endCharacter.ToString();

            switch (ch)
            {
                case -1:
                    throw new ArgumentException("End of input found when looking for escape character in single line string literal");

                case '"':
                    return "\"";

                case '\'':
                    return "'";

                case '\\':
                    return "\\";

                case 'a':
                    return "\a";

                case 'b':
                    return "\b";

                case 'f':
                    return "\f";

                case 't':
                    return "\t";

                case 'v':
                    return "\v";

                case 'n':
                    return "\n";

                case 'r':
                    if ((char)reader.Read() != '\\' || (char)reader.Read() != 'n')
                        throw new ArgumentException("CR found, but no matching LF found");
                    return "\n";

                case 'x':
                    return HexaCharacter(reader);

                default:
                    throw new ArgumentException("Invalid escape sequence found in string literal");
            }
        }

        /*
         * Reads a UNICODE character in a single string literal, starting out with
         * the '\x' characters.
         */
        static string HexaCharacter(StreamReader reader)
        {
            var builder = new StringBuilder();
            for (var idxNo = 0; idxNo < 4; idxNo++)
            {
                if (reader.EndOfStream)
                    throw new ArgumentException("EOF seen before escaped hex character was done reading");

                builder.Append((char)reader.Read());
            }
            var integerNo = Convert.ToInt32(builder.ToString(), 16);
            return Encoding.UTF8.GetString(BitConverter.GetBytes(integerNo).Reverse().ToArray());
        }

        #endregion
    }
}
