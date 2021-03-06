﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace magic.node.extensions.hyperlambda.internals
{
    /*
     * Helper class to help parse string literals, and other common tasks.
     */
    internal static class ParserHelper
    {
        /*
         * Reads a multline string literal, basically a string surrounded by @"".
         */
        internal static string ReadMultiLineString(StreamReader reader)
        {
            var builder = new StringBuilder();
            for (var c = reader.Read(); c != -1; c = reader.Read())
            {
                switch (c)
                {
                    case '"':
                        if ((char)reader.Peek() == '"')
                            builder.Append((char)reader.Read());
                        else
                            return builder.ToString();
                        break;

                    case '\n':
                        builder.Append("\r\n");
                        break;

                    case '\r':
                        if ((char)reader.Read() != '\n')
                            throw new ArgumentException(string.Format("Unexpected CR found without any matching LF near '{0}'", builder));
                        builder.Append("\r\n");
                        break;

                    default:
                        builder.Append((char)c);
                        break;
                }
            }
            throw new ArgumentException(string.Format("String literal not closed before end of input near '{0}'", builder));
        }

        /*
         * Reads a single line string literal, basically a string surrounded by only "".
         */
        internal static string ReadQuotedString(StreamReader reader)
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
         * Eats characters in stream reader, until the specified sequence is found.
         */
        internal static bool EatUntil(StreamReader reader, string sequence)
        {
            return EatUntil(reader, sequence, false);
        }

        #region [ -- Private helper methods -- ]

        static bool EatUntil(
            StreamReader reader,
            string sequence,
            bool recursed)
        {
            while (true)
            {
                if (reader.EndOfStream)
                    return false;

                var current = (char)reader.Peek();
                if (current == sequence.First())
                {
                    reader.Read(); // Discarding current character.
                    if (sequence.Length == 1 || EatUntil(reader, sequence.Substring(1), true))
                        return true; // Last character in sequence found.
                }
                else if (recursed)
                {
                    /*
                     * Notice, NOT moving stream pointer forward,
                     * since it still might be the first character in original sequence.
                     */
                    return false;
                }

                // Discarding current character, and moving to next character in stream.
                reader.Read();
            }
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
                    return "\r\n";

                default:
                    throw new ArgumentException("Invalid escape sequence found in string literal");
            }
        }

        #endregion
    }
}
