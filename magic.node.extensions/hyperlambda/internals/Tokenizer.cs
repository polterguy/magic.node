/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
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
        // Underlaying streamn, from where tokenizing process is conducted.
        readonly StreamReader _reader;

        /*
         * Dictionary containing functors for handling characters fetched
         * from the stream reader during parsing from a Hyperlambda stream.
         *
         * This architecture allows you to easily expand upon the tokenizer process,
         * and support 'custom tokens', for cases where you want to implement some
         * sort of LISP macro style type of injections, into the parsing process.
         */
        static readonly Dictionary<char, Func<StreamReader, StringBuilder, bool, string>> _characterFunctors =
        new Dictionary<char, Func<StreamReader, StringBuilder, bool, string>>
        {
            // Separates name and value of node (possibly).
            {':', HandleColonToken},

            // Starts the declaration of a multiline string literal (possibly).
            {'@', HandleAlphaToken},

            // Starts the declaration of a single line string literal (possibly).
            {'"', HandleDoubleQuoteToken},

            // Starts the declaration of a single line string literal (possibly).
            {'\'', HandleSingleQuoteToken},

            // Is the beginning of a CR/LF sequence.
            {'\r', HandleCRToken},

            // Is the entirety of a CR/LF sequence (Mac type of text files).
            {'\n', HandleLFToken},

            // Is the beginning of a single line or multi line comment.
            {'/', HandleSlashToken},

            // Is possibly a scope declaration for your node.
            {' ', HandleSPToken},
        };

        internal Tokenizer(StreamReader reader)
        {
            _reader = reader;
        }

        /*
         * Method responsible for actually retrieving tokens from stream.
         */
        internal IEnumerable<string> GetTokens(bool comments)
        {
            var builder = new StringBuilder();
            while (!_reader.EndOfStream)
            {
                var current = (char)_reader.Peek();
                if (_characterFunctors.ContainsKey(current))
                {
                    // We have a specialized functor for this character.
                    var result = _characterFunctors[current](_reader, builder, comments);
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

        #region [ -- Builtin tokenizer functors -- ]

        /*
         * Handles the ':' token, since it might be the separation of a node's value,
         * and its name.
         */
        static string HandleColonToken(StreamReader reader, StringBuilder builder, bool comments)
        {
            if (builder.Length == 0)
            {
                reader.Read(); // Discarding ':'.
                return ":";
            }
            var result = builder.ToString();
            builder.Clear();
            return result;
        }

        /*
         * Handles the '@' character, since it might imply a multiline string.
         */
        static string HandleAlphaToken(StreamReader reader, StringBuilder builder, bool comments)
        {
            if (builder.Length == 0)
            {
                reader.Read(); // Discarding '@'.
                var next = (char)reader.Read();
                if (next == '"')
                {
                    var result = ParserHelper.ReadMultiLineString(reader);
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
        }

        /*
         * Handles the '"' character, since it might imply a single line string.
         */
        static string HandleDoubleQuoteToken(StreamReader reader, StringBuilder builder, bool comments)
        {
            if (builder.Length == 0)
            {
                var result = ParserHelper.ReadQuotedString(reader);
                builder.Clear();
                return result;
            }
            builder.Append('"');
            reader.Read();
            return null;
        }

        /*
         * Handles the '\'' character, since it might imply a multiline string.
         */
        static string HandleSingleQuoteToken(StreamReader reader, StringBuilder builder, bool comments)
        {
            if (builder.Length == 0)
            {
                var result = ParserHelper.ReadQuotedString(reader);
                builder.Clear();
                return result;
            }
            builder.Append('\'');
            reader.Read();
            return null;
        }

        /*
         * Handles the '\r' character, assuming it's a part of a CR/LF sequence.
         */
        static string HandleCRToken(StreamReader reader, StringBuilder builder, bool comments)
        {
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
        }

        /*
         * Handles the '\n' character, handling it as a CR/LF sequence.
         */
        static string HandleLFToken(StreamReader reader, StringBuilder builder, bool comments)
        {
            if (builder.Length == 0)
            {
                reader.Read(); // Discarding '\n'.
                return "\r\n";
            }
            var result = builder.ToString();
            builder.Clear();
            return result;
        }

        /*
         * Handles the '/' character, since it might be the beginning of a comment,
         * either multiline comment or single line comment.
         */
        static string HandleSlashToken(StreamReader reader, StringBuilder builder, bool comments)
        {
            if (builder.Length == 0)
            {
                reader.Read(); // Discarding current '/'.
                var next = (char)reader.Peek();
                if (next == '/')
                {
                    if (comments)
                    {
                        // Semantically keeping comments around.
                        reader.Read(); // Discarding the last '/' character.
                        while ((char)reader.Peek() == ' ')
                            reader.Read(); // Discarding initial SP characters.

                        next = (char)reader.Peek();
                        while (next != '\r' && next != '\n' && !reader.EndOfStream)
                        {
                            builder.Append((char)reader.Read());
                            next = (char)reader.Peek();
                        }
                        return "..";
                    }
                    else
                    {
                        ParserHelper.EatUntil(reader, "\n");
                    }
                }
                else if (next == '*')
                {
                    if (comments)
                    {
                        // Semantically keeping comments around.
                        if (!ParserHelper.EatUntil(reader, "\n"))
                            throw new ArgumentException("Couldn't find end of multi line comment before EOF");
                        var seenAsterisk = false;
                        while (true)
                        {
                            if (reader.EndOfStream)
                                throw new ArgumentException("Couldn't find end of multi line comment before EOF");
                            var idx = (char)reader.Read();
                            if (idx == '*')
                            {
                                if ((char)reader.Peek() == '/')
                                {
                                    reader.Read();
                                    break;
                                }
                                else
                                {
                                    seenAsterisk = true;
                                    reader.Read();
                                }
                            }
                            else if (idx == '\n')
                            {
                                reader.Read(); // Discarding '\n'.
                                seenAsterisk = false;
                                builder.Append("\r\n");
                            }
                            else if (seenAsterisk && idx != '\r')
                            {
                                builder.Append(idx);
                            }
                        }
                        return "..";
                    }
                    else
                    {
                        if (!ParserHelper.EatUntil(reader, "*/"))
                            throw new ArgumentException("Couldn't find end of multi line comment before EOF");
                    }
                }
                else
                {
                    builder.Append('/'); // Only a part of the current token.
                }
            }
            else
            {
                // '/' is just a part of another token, and not a comment declaration.
                reader.Read(); // Discarding '/' character.
                builder.Append('/');
            }
            return null;
        }

        /*
         * Handles the ' ' token (SP), since it's probably the beginning
         * of a 'scope declaration'.
         */
        static string HandleSPToken(StreamReader reader, StringBuilder builder, bool comments)
        {
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
        }

        #endregion
    }
}
