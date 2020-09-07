/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace magic.node.extensions.hyperlambda.internals
{
    /*
     * Internal tokenizer class, for tokenizing a stream of characters into tokens
     * required for the Hyperlambda parser.
     */
    internal sealed class Tokenizer
    {
        readonly StreamReader _reader;

        public Tokenizer(StreamReader reader)
        {
            _reader = reader;
        }

        /*
         * Dictionary containing functors for handling characters fetched
         * from the stream reader during parsing from a Hyperlambda stream.
         */
        static readonly Dictionary<char, Func<StreamReader, StringBuilder, string>> _characterFunctors = new Dictionary<char, Func<StreamReader, StringBuilder, string>>
        {
            {':', (reader, builder) => {
                if (builder.Length == 0)
                {
                    reader.Read(); // Discarding ':'.
                    return ":";
                }
                var result = builder.ToString();
                builder.Clear();
                return result;
            }},
            {'@', (reader, builder) => {
                if (builder.Length == 0)
                {
                    reader.Read(); // Discarding '@'.
                    var next = (char)reader.Read();
                    if (next == '"')
                    {
                        var result = StringLiteralParser.ReadMultiLineString(reader);
                        builder.Clear();
                        return result;
                    }
                    builder.Append('@').Append(next);
                }
                else
                {
                    builder.Append('@');
                    reader.Read();
                }
                return null;
            }},
            {'"', (reader, builder) => {
                if (builder.Length == 0)
                {
                    var result = StringLiteralParser.ReadQuotedString(reader);
                    builder.Clear();
                    return result;
                }
                builder.Append('"');
                reader.Read();
                return null;
            }},
            {'\'', (reader, builder) => {
                if (builder.Length == 0)
                {
                    var result = StringLiteralParser.ReadQuotedString(reader);
                    builder.Clear();
                    return result;
                }
                builder.Append('\'');
                reader.Read();
                return null;
            }},
            {'\r', (reader, builder) => {
                if (builder.Length == 0)
                {
                    reader.Read(); // Discarding '\r'.
                    if (reader.EndOfStream || (char)reader.Read() != '\n')
                        throw new ArgumentException("CR/LF error in Hyperlambda");
                    return "\r\n";
                }
                var result = builder.ToString();
                builder.Clear();
                return result;
            }},
            {'\n', (reader, builder) => {
                if (builder.Length == 0)
                {
                    reader.Read(); // Discarding '\n'.
                    return "\r\n";
                }
                var result = builder.ToString();
                builder.Clear();
                return result;
            }},
            {'/', (reader, builder) => {
                if (builder.Length == 0)
                {
                    reader.Read(); // Discarding current '/'.
                    if (reader.Peek() == '/')
                    {
                        while (!reader.EndOfStream && (char)reader.Peek() != '\n')
                            reader.Read();
                    }
                    else if (reader.Peek() == '*')
                    {
                        // Eating until "*/".
                        var seenEndOfComment = false;
                        while (!reader.EndOfStream && !seenEndOfComment)
                        {
                            var idxComment = reader.Read();
                            if (idxComment == '*' && reader.Peek() == '/')
                            {
                                reader.Read();
                                seenEndOfComment = true;
                            }
                        }
                        if (!seenEndOfComment && reader.EndOfStream)
                            throw new ArgumentException("Syntax error in comment close to end of Hyperlambda");
                    }
                    else
                    {
                        builder.Append('/'); // Only a part of the current token.
                    }
                }
                else
                {
                    reader.Read(); // Discarding '/' character.
                    builder.Append('/');
                }
                return null;
            }},
            {' ', (reader, builder) => {
                reader.Read(); // Discarding current ' '.
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                    return null;
                }
                builder.Append(' ');
                while (!reader.EndOfStream && (char)reader.Peek() == ' ')
                {
                    reader.Read();
                    builder.Append(' ');
                }
                if (!reader.EndOfStream && builder.Length % 3 != 0)
                    throw new ArgumentException("Odd number of spaces found in Hyperlambda file");
                var result = builder.ToString();
                builder.Clear();
                return result;
            }},
        };

        /*
         * Method responsible for actually retrieving tokens from stream.
         */
        internal IEnumerable<string> GetTokens()
        {
            var builder = new StringBuilder();
            while (!_reader.EndOfStream)
            {
                var current = (char)_reader.Peek();
                if (_characterFunctors.ContainsKey(current))
                {
                    var result = _characterFunctors[current](_reader, builder);
                    if (result != null)
                        yield return result;
                }
                else
                {
                    builder.Append(current);
                    _reader.Read();
                }
            }

            // Returning the last token, if any.
            if (builder.Length > 0)
                yield return builder.ToString();
        }
    }
}
