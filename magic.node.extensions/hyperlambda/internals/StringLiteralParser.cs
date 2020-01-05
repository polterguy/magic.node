/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace magic.node.extensions.hyperlambda.internals
{
    /*
     * Helper class to help parse string literals.
     */
    internal static class StringLiteralParser
    {
        /*
         * Reads a multline string literal, basically a string surrounded by @"".
         */
        public static string ReadMultiLineString(StreamReader reader)
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
                            throw new Exception(string.Format("Unexpected CR found without any matching LF near '{0}'", builder));
                        builder.Append("\r\n");
                        break;

                    default:
                        builder.Append((char)c);
                        break;
                }
            }
            throw new Exception(string.Format("String literal not closed before end of input near '{0}'", builder));
        }

        /*
         * Reads a single line string literal, basically a string surrounded by only "".
         */
        public static string ReadQuotedString(StreamReader reader)
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
                        throw new ApplicationException(string.Format("Syntax error, string literal unexpected CR/LF"));

                    default:
                        builder.Append((char)c);
                        break;
                }
            }
            throw new ApplicationException(string.Format("Syntax error, string literal not closed before end of input"));
        }

        #region [ -- Private helper methods -- ]

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
                    throw new Exception("End of input found when looking for escape character in single line string literal");

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
                        throw new Exception("CR found, but no matching LF found");
                    return "\n";

                case 'x':
                    return HexaCharacter(reader);

                default:
                    throw new Exception("Invalid escape sequence found in string literal");
            }
        }

        /*
         * Reads a UNICODE character in a single string literal, starting out with
         * the '\x' characters.
         */
        static string HexaCharacter(StreamReader reader)
        {
            var hexNumberString = "";
            for (var idxNo = 0; idxNo < 4; idxNo++)
            {
                if (reader.EndOfStream)
                    throw new ApplicationException("EOF seen before escaped hex character was done reading");

                hexNumberString += (char)reader.Read();
            }
            var integerNo = Convert.ToInt32(hexNumberString, 16);
            return Encoding.UTF8.GetString(BitConverter.GetBytes(integerNo).Reverse().ToArray());
        }

        #endregion
    }
}
